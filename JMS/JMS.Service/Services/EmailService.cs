using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using JMS.ViewModels.Register;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using JMS.Service.Settings;
using Microsoft.Extensions.Configuration;

namespace JMS.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IServiceProvider _serviceProvider; 
        public EmailService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task SendEmailConfirmationMail(ApplicationUser user)
        {
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var token = await _serviceProvider.GetService<UserManager<ApplicationUser>>().GenerateEmailConfirmationTokenAsync(user);
            var emailBody = await _serviceProvider.GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/EmailTemplates/ConfirmEmail.cshtml", new ConfirmEmailModel { UserId = user.Id, Token = token });
            _serviceProvider.GetService<IEmailSender>().SendEmail(new MailMessage(configuration[JMSSetting.SenderEmail], user.Email, configuration[JMSSetting.ResetPasswordSubject], emailBody) { IsBodyHtml = true });
        }
    }
}
