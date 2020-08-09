using JMS.Entity.Entities;
using JMS.ViewModels.Register;
using JMS.ViewModels.SystemAdmin;
using JMS.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        Task<UserGridModel> GetJournalUsers(string tenantId, UserGridSearchModel userGridSearchModel);
        Task CreateUser(string tenantId, CreateUserViewModel createUserViewModel);
        Task SaveUser(string path, EditUserViewModel editUserViewModel);
        Task DeleteUser(string path,long userId);

        Task<ApplicationUser> CreateAuthor(string path, RegisterAuthorModel model);
        Author GetAuthor(long userId);
        void SaveAuthor(AuthorProfileModel authorProfileModel, Stream fileStream, string fileName);
        IDictionary<long, string> GetJounalEditors(string journalPath);
    }
}
