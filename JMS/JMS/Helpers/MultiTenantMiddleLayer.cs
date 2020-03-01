using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Helpers
{
    public class MultiTenantMiddleLayer
    {
        private readonly RequestDelegate _next;
        public MultiTenantMiddleLayer(RequestDelegate next)
        {
            _next = next;
           
        }
        public async Task Invoke(HttpContext httpContext, ITenantService tenantService, IConfiguration configuration)
        {
            var tenants = tenantService.GetTenantPaths().ToList();
            tenants.Add(configuration[JMSSetting.DefaultTenant]);
            var path = httpContext.Request.Path.Value;
            if (path != "/")
            {
                if (!tenants.Contains(httpContext.Request.Path.Value.Split("/", StringSplitOptions.RemoveEmptyEntries)[0]))
                {
                    httpContext.Response.StatusCode = 404;
                }
                else
                {
                    await _next(httpContext);
                }
            }
            else
            {
                httpContext.Response.Redirect($"/{configuration[JMSSetting.DefaultTenant]}");
            }
            
        }
    }
}
