using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JMS.Helpers;
using JMS.Models.Admin;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using JMS.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.Admin)]
    public class AdminController : BaseController
    {
        private readonly ITenantService _tenantService;
        private readonly IFileService _fileService;
        public AdminController(ITenantService tenantService, IFileService fileService)
        {
            _tenantService = tenantService;
            _fileService = fileService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult JournalSettings()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tenant = _tenantService.GetTenantByUserId(userId);
            var model = new Models.Admin.AdminJournalSettingModel
            {
                IsActive = !tenant.IsDisabled.GetValueOrDefault(),
                JournalName = tenant.JournalName,
                JournalTitle = tenant.JournalTitle,
                LogoPath = _fileService.GetFile(tenant.JournalLogo),
                OnlneISSNNumber = tenant.OnlineISSN,
                PrintISSNNumber = tenant.PrintISSN,
                Publisher = tenant.Publisher
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult JournalSettings(Models.Admin.AdminJournalSettingModel model)
        {
            if (ModelState.IsValid)
            {
                _tenantService.SaveTenant(new ViewModels.Admin.AdminJournalSettingModel
                {
                    IsActive = model.IsActive,
                    JournalName = model.JournalName,
                    JournalTitle = model.JournalTitle,
                    OnlneISSNNumber = model.OnlneISSNNumber,
                    PrintISSNNumber = model.PrintISSNNumber,
                    Publisher = model.Publisher,
                    UserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                }, model.Logo?.OpenReadStream(), model.Logo?.FileName);
                TempData.Add(Messages.SuccessJournalSettingMessage, Messages.SuccessJournalSettingMessage);
                return RedirectToAction("JournalSettings");
            }
            return View();
        }

        public IActionResult ContactSettings()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tenant = _tenantService.GetTenantByUserId(userId);
            var model = new JournalContactSettingModel
            {
                Address = tenant.FirstLine,
                City = tenant.City,
                Country = tenant.Country,
                Email = tenant.Email,
                Phone = tenant.PhoneNumber,
                State = tenant.State,
                Zip = tenant.Zip
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactSettings(JournalContactSettingModel model)
        {
            if (ModelState.IsValid)
            {
                _tenantService.SaveTenantContactSetting(model, long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                TempData.Add(Messages.SuccessContactSettingMessage, Messages.SuccessContactSettingMessage);
                return RedirectToAction("ContactSettings");
            }
            return View();
        }

        public IActionResult AppearanceSettings()
        {
            var model = new AppearanceSettingsModel
            {
                AboutUsContent = _tenantService.AboutUsContent(TenantID),
                AdditionalContent = _tenantService.AdditionalContent(TenantID),
                FooterContent = _tenantService.FooterContent(TenantID),
                PrivacyPolicyContent = _tenantService.PrivacyPolicyContent(TenantID)
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AppearanceSettings(AppearanceSettingsModel model)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _tenantService.SaveAppearanceSettings(model, userId);
            if (Request.IsAjaxRequest())
            {
                return Ok();
            }
            return RedirectToAction("AppearanceSettings");
        }
    }
}