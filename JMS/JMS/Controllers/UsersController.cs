using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using JMS.ViewModels.Users;
using JMS.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using JMS.Entity.Entities;
using JMS.Service.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using JMS.Models.EmailModels;
using System.Net.Mail;
using JMS.Service.Settings;
using ElmahCore;

namespace JMS.Controllers
{
    public class UsersController : BaseController
    {
        [Authorize(Roles = RoleName.Admin)]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = RoleName.Admin)]
        public async Task<UserGridModel> JournalUsers([FromQuery]UserGridSearchModel userGridSearchModel)
        {
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            var users = await userService.GetJournalUsers(TenantID, userGridSearchModel);
            return users;
        }
        [Authorize(Roles = RoleName.Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = RoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Users.CreateUserViewModel createUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var userService = HttpContext.RequestServices.GetService<IUserService>();
                var mapper = HttpContext.RequestServices.GetService<IMapper>();
                await userService.CreateUser(TenantID, mapper.Map<Models.Users.CreateUserViewModel, ViewModels.Users.CreateUserViewModel>(createUserViewModel));
                var emailModel = new AddUserEmailNotificationModel
                {
                    FirstName = createUserViewModel.FirstName,
                    LastName = createUserViewModel.LastName,
                    JournalName = GetService<ITenantService>().GetTenant(JMSUser.TenantId.Value).JournalName,
                    JournalPath = TenantID,
                    RoleNames = createUserViewModel.Roles.Select(x => ((Role)x).ToString()).ToList()
                };
                try
                {
                    await SendAddUserEmail(emailModel, createUserViewModel.Email);
                }
                catch (System.Exception ex)
                {
                    HttpContext.RiseError(ex);
                    AddFailMessage("User has been Added  but failed to send confirmation email to user.");
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = RoleName.Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            var roleManager = HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
            var user = userService.GetUser(id);
            var roles = await roleManager.GetRolesAsync(user);
            
            var existingRoles = RoleHelper.GetRolesForUser().Select(x => new SelectListItem(x.Value, x.Key.ToString(), roles.Contains(x.Value)));
            var model = new Models.Users.EditUserViewModel
            {
                UserId = id,
                FirstName = user.FirstName,
                Email = user.Email,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = !user.IsDisabled.GetValueOrDefault(),
                AssignedRoles = existingRoles,
                AffiliationNo = user.AffiliationNo,
                IsJournalAdmin = roles.Contains(Role.JournalAdmin.ToString()),
                ReviewCount= userService.AssignedSubmissionAsReviewer(user.Id,TenantID),
                EditorCount= userService.AssignedSubmissionAsEditor(user.Id,TenantID)
            };
            return View(model);
        }

        [Authorize(Roles = RoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ViewModels.Users.EditUserViewModel editUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var userService = HttpContext.RequestServices.GetService<IUserService>();
                await userService.SaveUser(TenantID, editUserViewModel);
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = RoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(long Id)
        {
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            await userService.DeleteUser(TenantID, Id);
            return RedirectToAction("Index");
        }

        private async Task SendAddUserEmail(AddUserEmailNotificationModel emailModel, string email)
        {
            var emailBody = await GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/EmailTemplates/AddUserNotification.cshtml", emailModel);
            var mailMessage = new MailMessage(_configuration[JMSSetting.SenderEmail], email, _configuration[JMSSetting.AccountConfirmationEmailSubject], emailBody) { IsBodyHtml = true };
            GetService<IEmailSender>().SendEmail(mailMessage);
        }
    }
}