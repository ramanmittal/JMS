using JMS.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface IUserService
    {
        IEnumerable<ApplicationUser> GetTenantUserByRole(long tenantid, string role);
        ApplicationUser GetUser(long userId);
        bool ValidateEmail(string email, long tenantId, long? userid);
    }
}
