
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace JMS.Service.Services
{
    public class SMTPEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public SMTPEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(MailMessage mailMessage)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var smtp = new SmtpClient
            {
                Host = _configuration[JMSSetting.SMTPServer],
                Port = int.Parse(_configuration[JMSSetting.SMTPPort]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration[JMSSetting.SMTPUserName], _configuration[JMSSetting.SMTPPassword])
            };
            smtp.Send(mailMessage);
        }
    }
}
