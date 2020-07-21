using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace JMS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IConfiguration _configuration { get { return HttpContext.RequestServices.GetService<IConfiguration>(); } }
       
        public string TenantID
        {
            get
            {
                var tenantId = Request.RouteValues["tenant"].ToString();
                return tenantId.Equals( _configuration[JMSSetting.DefaultTenant],StringComparison.CurrentCultureIgnoreCase) ? null : tenantId;
            }
        }

        protected void AddSuccessMessage(string message)
        {
            List<string> messages = null;
            if (TempData.ContainsKey(JMS.Setting.Messages.success))
            {
                messages = (List<string>)TempData[JMS.Setting.Messages.success];
            }
            else
            {
                messages = new List<string>();
                TempData.Add(JMS.Setting.Messages.success, messages);
            }
            messages.Add(message);
        }
        
    }
}