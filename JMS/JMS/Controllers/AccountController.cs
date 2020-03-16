﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using JMS.Setting;
using JMS.Models.Account;
using JMS.ViewModels.SystemAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        public AccountController(IConfiguration configuration, IUserService userservice, IAccountService accountService, IEmailSender emailSender, IFileService fileService, SignInManager<ApplicationUser> signInManager, IRazorViewToStringRenderer razorViewToStringRenderer) : base(configuration)
        {
            _userService = userservice;
            _accountService = accountService;
            _emailSender = emailSender;
            _fileService = fileService;
            _signInManager = signInManager;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }
        [AllowAnonymous]
        public IActionResult ValidateEmail(string email, long tenantId, long? userid)
        {
            if (!_userService.ValidateEmail(email, tenantId, userid))
            {
                return Json(data: Messages.EmailNotAvailiable);
            }
            return Json(data: true);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(viewName:string.IsNullOrEmpty(TenantID)? "Login" : "Login.journal");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email, TenantID);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Rememberme, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData.Add(Messages.InvalidLoginAttempt, Messages.InvalidLoginAttempt);
            }
            return View(viewName: string.IsNullOrEmpty(TenantID) ? "Login" : "Login.journal");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForGotPassword()
        {
            return View(viewName: string.IsNullOrEmpty(TenantID) ? "ForGotPassword" : "ForGotPassword.journal");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForGotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await _accountService.GetResetPasswordTokenByEmail(model.Email, TenantID);
                if (token != null)
                {
                    var emailBody = await _razorViewToStringRenderer.RenderViewToStringAsync(@"/Views/EmailTemplates/ResetPassword.cshtml", new ForgotpasswordEmailModel { Email = model.Email, Token = token });
                    _emailSender.SendEmail(new MailMessage(_configuration[JMSSetting.SenderEmail], model.Email, _configuration[JMSSetting.ResetPasswordSubject], emailBody) { IsBodyHtml = true });
                }
                TempData.Add(Messages.SuccessPasswordRecoverEmailMessage, Messages.SuccessPasswordRecoverEmailMessage);
            }
            return View(viewName: string.IsNullOrEmpty(TenantID) ? "ForGotPassword" : "ForGotPassword.journal");
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
                return View(viewName: string.IsNullOrEmpty(TenantID) ? "ResetPassword" : "ResetPassword.journal");
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
                var result = await _accountService.ResetPassword(model.Email, model.Token, model.Password, TenantID);
                if (result.Succeeded)
                {
                    var user = _userService.GetUserByEmail(model.Email, TenantID);
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home"); 
                }
                ModelState.AddModelError("Password", result.Errors.First().Description);
            }
            return View(viewName: string.IsNullOrEmpty(TenantID) ? "ResetPassword" : "ResetPassword.journal");
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

        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword changePasswordmodel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ChangePassword(long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), changePasswordmodel.Password, changePasswordmodel.ConfirmPassword);
                if (result.Succeeded)
                {
                    TempData.Add(Messages.SuccessPasswordChangeMessage, Messages.SuccessPasswordChangeMessage);
                    ModelState.Clear();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Password", error.Description);
                    }
                }
            }
            return View();
        }
    }
}