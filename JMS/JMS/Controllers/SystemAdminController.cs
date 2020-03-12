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
        private readonly IAccountService _accountService;
        private readonly ISystemService _systemService;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SystemAdminController(IAccountService accountService, IRazorViewToStringRenderer razorViewToStringRenderer, IEmailSender emailSender, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, ISystemService systemService, IUserService userService, IFileService fileService) : base(configuration)
        {
            _accountService = accountService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _systemService = systemService;
            _userService = userService;
            _fileService = fileService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel  model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Rememberme, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForGotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForGotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await _accountService.GetResetPasswordTokenByEmail(model.Email,TenantID);
                if (token != null)
                {
                    var emailBody = await _razorViewToStringRenderer.RenderViewToStringAsync(@"/Views/EmailTemplates/ResetPassword.cshtml", new ForgotpasswordEmailModel { Email = model.Email, Token = token });
                    _emailSender.SendEmail(new MailMessage(_configuration[JMSSetting.SenderEmail], model.Email, _configuration[JMSSetting.ResetPasswordSubject], emailBody) { IsBodyHtml = true });
                }
            }
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(ForgotpasswordEmailModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _accountService.VerifyUserTokenAsync(model.Email, model.Token, TenantID))
                {
                    return View(new ResetPasswordModel { Email = model.Email, Token = model.Token });
                }
                return View();
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountService.ResetPassword(model.Email, model.Token, model.Password, TenantID);
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index");
            }
            return View();
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

        public IActionResult ViewProfile()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _userService.GetUser(userId);
            return View(new JMS.Models.SystemAdmin.SystemAdminProfileModel
            {
                City = user.City,
                Country = user.Country,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                State = user.State,
                Zip = user.Zip,
                ProfileImagePath = string.IsNullOrEmpty(user.ProfileImage) ? null : _fileService.GetFile(user.ProfileImage)
            }); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProfile(JMS.Models.SystemAdmin.SystemAdminProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userService.SaveSystemAdmin(new SystemAdminProfileModel
                {
                    City = model.City,
                    Country = model.Country,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    State = model.State,
                    UserId = userId,
                    Zip = model.Zip
                }, model.ProfileImageFile?.OpenReadStream(), model.ProfileImageFile?.FileName);
                TempData.Add(Messages.SuccessProfileMessage, Messages.SuccessProfileMessage);
                return RedirectToAction("ViewProfile");
            }
            return View("ViewProfile", model);
        }
    }
}