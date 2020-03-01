using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IConfiguration _configuration;
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string TenantID
        {
            get
            {
                var tenantId = Request.RouteValues["tenant"].ToString();
                return tenantId.Equals( _configuration[JMSSetting.DefaultTenant],StringComparison.CurrentCultureIgnoreCase) ? null : tenantId;
            }
        }
    }
}