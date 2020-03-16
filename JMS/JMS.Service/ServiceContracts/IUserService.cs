using JMS.Entity.Entities;
using JMS.ViewModels.SystemAdmin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface IUserService
    {
        IEnumerable<ApplicationUser> GetTenantUserByRole(long tenantid, string role);
        ApplicationUser GetUser(long userId);
        bool ValidateEmail(string email, long tenantId, long? userid);
        void SaveSystemAdmin(SystemAdminProfileModel systemAdminProfileModel, Stream fileStream, string fileName);
        string GetUserProfileImage(long userid);
        string GetUserProfileImage(ApplicationUser user);
        ApplicationUser GetUserByEmail(string email, string path);
    }
}
