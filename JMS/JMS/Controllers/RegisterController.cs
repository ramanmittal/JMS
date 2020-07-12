using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;
using ElmahCore;
using JMS.Entity.Entities;
using JMS.Models.Register;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using JMS.ViewModels.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace JMS.Controllers
{
    public class RegisterController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Models.Register.RegisterAuthorModel model)
        {
            if (ModelState.IsValid)
            {
                var registerAuthorModel = HttpContext.RequestServices.GetService<IMapper>().Map<JMS.ViewModels.Register.RegisterAuthorModel>(model);
                var user = await HttpContext.RequestServices.GetService<IUserService>().CreateAuthor(TenantID, registerAuthorModel);
                var token = await HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>().GenerateEmailConfirmationTokenAsync(user);
                var emailBody = await HttpContext.RequestServices.GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/EmailTemplates/ResetPassword.cshtml", new ConfirmEmailModel { UserId = user.Id, Token = token });
                try
                {
                    HttpContext.RequestServices.GetService<IEmailSender>().SendEmail(new MailMessage(_configuration[JMSSetting.SenderEmail], model.Email, _configuration[JMSSetting.ResetPasswordSubject], emailBody) { IsBodyHtml = true });
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(ex);
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}