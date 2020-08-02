using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using JMS.Service.Services;
using JMS.Service.Settings;
using JMS.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;
using JMS.Services;
using JMS.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using JMS.Infra.Sequrity;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ElmahCore.Sql;
using ElmahCore.Postgresql;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace JMS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IHttpContextAccessor httpContextAccessor;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("JMS")));
            services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            var mvc = services.AddControllersWithViews();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorPolicyRequirementHandler.AuthorPolicy, policy =>
                    policy.Requirements.Add(new AuthorPolicyRequirement()));
            });
#if (DEBUG)
            mvc.AddRazorRuntimeCompilation();
#endif
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<IEmailSender, SMTPEmailSender>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMaskService, Maskservice>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.RemoveAll<IUserValidator<ApplicationUser>>();
            services.TryAddScoped<IUserValidator<ApplicationUser>, JMSUserValidator>();
            services.AddSingleton<IFileService>(x =>
            {
                var hostingEnv = x.GetService<IWebHostEnvironment>();
                return new LocalFileService(Path.Combine(hostingEnv.WebRootPath, @"uploaded"), @"/uploaded/");
            });
            services.AddRazorPages();
            services.ConfigureApplicationCookie(option => {
                var defaultcookie = option.Cookie;                
                option.Cookie = new JMSCookiesBuilder(httpContextAccessor);               
                option.Cookie.Domain = defaultcookie.Domain;
                option.Cookie.Expiration = defaultcookie.Expiration;
                option.Cookie.HttpOnly = defaultcookie.HttpOnly;
                option.Cookie.IsEssential = defaultcookie.IsEssential;
                option.Cookie.MaxAge = defaultcookie.MaxAge;
                if (!string.IsNullOrEmpty(defaultcookie.Name))
                    option.Cookie.Name = defaultcookie.Name;
                option.Cookie.SameSite = defaultcookie.SameSite;
                option.Cookie.SecurePolicy = defaultcookie.SecurePolicy;
                option.EventsType = typeof(MyCookieAuthenticationEvents);
            });
            services.AddScoped<MyCookieAuthenticationEvents>();
            services.Configure<UserMenu>(Configuration.GetSection("Menus"));
            services.AddAutoMapper(GetType().Assembly);
            services.AddScoped<IAuthenticationService, JMSAuthenticationService>();
            services.AddSingleton<IAuthorizationHandler, PreventDisableUserHandler>();
            services.AddSingleton<IAuthorizationHandler, AuthorPolicyRequirementHandler>();
            services.AddScoped<ISMSService, TwilioSmsService>();
            services.AddElmah<PgsqlErrorLog>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("JMS");
            });
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseElmah();
           
            app.UseMiddleware<MultiTenantMiddleLayer>();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "jms",
                 pattern: $"{Configuration[JMSSetting.DefaultTenant]}/systemsettings",
                 defaults: new { tenant = Configuration[JMSSetting.DefaultTenant], controller = "SystemAdmin", action = "Settings" }
                 );
                
                endpoints.MapControllerRoute(
                  name: "InitializeJMS",
                  pattern: $"{Configuration[JMSSetting.DefaultTenant]}/InitializeJMS",
                  defaults: new { controller = "Home", action = "InitializeJMS" }
                  );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{tenant}/{controller=Home}/{action=Index}/{id?}");


                endpoints.MapRazorPages();
            });
        }
    }
}
