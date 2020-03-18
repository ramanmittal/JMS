
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
            //SmtpClient SmtpServer = new SmtpClient("in-v3.mailjet.com");
            //SmtpServer.Port = 25;
            //SmtpServer.Credentials =
            //new System.Net.NetworkCredential("7a5c11676881b862838cb6fd94734ed0", "dabc4fd93401d6f8a88ceb5d0f09d137");
            //SmtpServer.EnableSsl = true;
            //SmtpServer.Send(mailMessage);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

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
