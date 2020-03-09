
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface IEmailSender
    {
        void SendEmail(MailMessage mailMessage);
    }
}
