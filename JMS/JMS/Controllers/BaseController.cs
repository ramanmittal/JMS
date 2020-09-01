using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using JMS.Infra.Sequrity;
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
        protected void AddFailMessage(string message)
        {
            List<string> messages = null;
            if (TempData.ContainsKey(JMS.Setting.Messages.fail))
            {
                messages = (List<string>)TempData[JMS.Setting.Messages.fail];
            }
            else
            {
                messages = new List<string>();
                TempData.Add(JMS.Setting.Messages.fail, messages);
            }
            messages.Add(message);
        }

        protected T GetService<T>()
        {
            return HttpContext.RequestServices.GetService<T>();
        }

        protected  ApplicationUser JMSUser
        {
            get
            {
                return ((JMSPrincipal)User).ApplicationUser;
            }
        }
        
    }
}