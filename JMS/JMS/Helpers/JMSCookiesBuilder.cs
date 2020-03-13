using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Helpers
{
    public class JMSCookiesBuilder : CookieBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JMSCookiesBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            //this.Domain = cookieBuilder.Domain;
            //this.Expiration = cookieBuilder.Expiration;
            //this.HttpOnly = cookieBuilder.HttpOnly;
            //this.IsEssential = cookieBuilder.IsEssential;
            //this.MaxAge = cookieBuilder.MaxAge;
            //this.Name = cookieBuilder.Name;
            //this.SameSite = cookieBuilder.SameSite;
            //this.SecurePolicy = cookieBuilder.SecurePolicy;

        }
        public override string Path
        {
            get
            {
               return  @"/"+_httpContextAccessor.HttpContext.Request.RouteValues["tenant"].ToString();
            }
            set { }            
        }
    }

    public class MyCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> redirectContext)
        {
            redirectContext.RedirectUri = $"/{redirectContext.Request.RouteValues["tenant"].ToString()}/Account/Login";
            return base.RedirectToLogin(redirectContext);
        }
    }
}
