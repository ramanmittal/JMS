using JMS.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JMS.Service.ServiceContracts
{
    public interface IEmailService
    {
        Task SendEmailConfirmationMail(ApplicationUser user);
    }
}
