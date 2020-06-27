using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using JMS.Service.Enums;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using JMS.Setting;
using JMS.ViewModels.SystemAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.SystemAdmin)]
    public class SystemAdminController : BaseController
    {
        
        private readonly ISystemService _systemService;        
       
        public SystemAdminController(ISystemService systemService)
        {  
            _systemService = systemService;            
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index","Journals");
        }
        public IActionResult Settings()
        {
            var model = _systemService.GetSystemSettings();
            var m1 = new JMS.Models.SystemAdmin.SystemSettingViewModel
            {
                SystemTitle = model.SystemTitle,
                SystemLogo = model.SystemLogo
            };
            return View(m1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Settings(JMS.Models.SystemAdmin.SystemSettingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var m1 = new SystemSettingViewModel
                {
                    SystemTitle = model.SystemTitle,
                    SystemLogo = model.SystemLogo
                };
                _systemService.SetSystemSetting(m1, model.SystemLogoFile?.OpenReadStream(), model.SystemLogoFile?.FileName);
                TempData.Add(Messages.SuccessSettingMessage, Messages.SuccessSettingMessage);
                m1 = _systemService.GetSystemSettings();
                model = new JMS.Models.SystemAdmin.SystemSettingViewModel
                {
                    SystemTitle = m1.SystemTitle,
                    SystemLogo = m1.SystemLogo
                };
            }            
            return View(model);
        }

        
    }
}