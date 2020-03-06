using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
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
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.IO;

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
                options.UseSqlServer(
                    Configuration.GetConnectionString("JMS")));
            services.AddIdentity<ApplicationUser, IdentityRole<long>>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            var mvc = services.AddControllersWithViews(
                //options => options.Filters.Add<MultiTenantActionFilter>()
                );
#if (DEBUG)
            mvc.AddRazorRuntimeCompilation();
#endif
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<IEmailSender, LogEmailSender>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddSingleton<IFileService>(x =>
            {
                var hostingEnv = x.GetService<IHostingEnvironment>();
                return new LocalFileService(Path.Combine(hostingEnv.WebRootPath, @"img\uploaded\Journal-logo"), @"/img/uploaded/Journal-logo/");
            });
            services.AddRazorPages();
            //services.ConfigureApplicationCookie(option => option.Cookie.Path = @"/jms");
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
            });

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
            app.UseMiddleware<MultiTenantMiddleLayer>();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "jms",
                  pattern: "jms",
                  defaults: new { tenant = Configuration[JMSSetting.DefaultTenant], controller = "SystemAdmin", action = "Login" }
                  );
                endpoints.MapControllerRoute(
                  name: "InitializeJMS",
                  pattern: "InitializeJMS",
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
