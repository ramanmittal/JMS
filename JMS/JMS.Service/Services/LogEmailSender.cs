using JMS.Service.ServiceContracts;
using System;
using System.Net.Mail;

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
