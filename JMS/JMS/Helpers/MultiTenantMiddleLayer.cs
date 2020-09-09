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
            var tenants = tenantService.GetTenantPathWithStatus();
            tenants.Add(configuration[JMSSetting.DefaultTenant], false);
            var path = httpContext.Request.Path.Value;
            if (path != "/")
            {
                var tenantpath = (httpContext.Request.Path.Value.Split("/", StringSplitOptions.RemoveEmptyEntries)[0]);
                var tenant = tenants.FirstOrDefault(x => x.Key.Equals(tenantpath, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(tenant.Key))
                {
                    httpContext.Response.StatusCode = 404;
                    await httpContext.Response.CompleteAsync();
                    return;
                }
                else
                {
                    if (tenant.Value.GetValueOrDefault())
                    {
                        httpContext.Response.StatusCode = 503;
                        await httpContext.Response.CompleteAsync();
                        return;
                    }
                    if (tenantpath != tenant.Key)
                        httpContext.Request.Path = new PathString(ReplaceFirstOccurrence(httpContext.Request.Path.Value, tenantpath, tenant.Key));
                    await _next(httpContext);
                }
            }
            else
            {
                httpContext.Response.Redirect($"/{configuration[JMSSetting.DefaultTenant]}");
            }
            
        }
        public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.IndexOf(Find);
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }
    }
}
