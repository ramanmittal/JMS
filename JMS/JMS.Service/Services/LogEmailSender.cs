using JMS.Service.ServiceContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;

namespace JMS.Service.Services
{
    public class LogEmailSender : IEmailSender
    {
        private readonly ILogger<LogEmailSender> _logger;
        public LogEmailSender(ILogger<LogEmailSender> logger)
        {
            _logger = logger;
        }
        public void SendEmail(MailMessage mailMessage)
        {
            _logger.LogInformation(mailMessage.Body);
        }
    }
}
