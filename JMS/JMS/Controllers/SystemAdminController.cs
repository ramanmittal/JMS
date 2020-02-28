using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using JMS.Service.Enums;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using JMS.Setting;
using JMS.ViewModels.SystemAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.SystemAdmin)]
    public class SystemAdminController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public SystemAdminController(IAccountService accountService, IRazorViewToStringRenderer razorViewToStringRenderer, IEmailSender emailSender, IConfiguration configuration)
        {
            _accountService = accountService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _emailSender = emailSender;
            _configuration = configuration;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel  model)
        {
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
                var token = await _accountService.GetResetPasswordTokenByEmail(model.Email);
                var emailBody = await _razorViewToStringRenderer.RenderViewToStringAsync(@"/Views/EmailTemplates/ResetPassword.cshtml", token);
                _emailSender.SendEmail(new MailMessage(_configuration[JMSSetting.SenderEmail], model.Email, _configuration[JMSSetting.ResetPasswordSubject], emailBody) { IsBodyHtml = true });
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            return View();
        }
    }
}