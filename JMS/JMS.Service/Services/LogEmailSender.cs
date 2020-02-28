using JMS.Service.ServiceContracts;
using Microsoft.Extensions.Logging;
using ServiceModel;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace JMS.Service.Services
{
    public class LogEmailSender : IEmailSender
    {        
        public void SendEmail(MailMessage mailMessage)
        {
            Console.WriteLine(mailMessage.Body);
        }
    }
}
