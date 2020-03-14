using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using JMS.ViewModels.SystemAdmin;
using System.IO;
using Microsoft.Extensions.Configuration;
using JMS.Service.Settings;

namespace JMS.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly IMaskService _maskService;
        public UserService(ApplicationDbContext context, IFileService fileService, IConfiguration configuration, IMaskService maskService)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _maskService = maskService;
        }
        public IEnumerable<ApplicationUser> GetTenantUserByRole(long tenantid, string role)
        {
            var users = _context.Users.Where(x => x.TenantId == tenantid);
            if (!string.IsNullOrEmpty(role))
            {
                var roleid = _context.Roles.Single(x => x.Name == role).Id;
                var roledUser = _context.UserRoles.Where(x => x.RoleId == roleid);
                users = from userRole in roledUser
                        join
                        user in users
                        on userRole.UserId equals user.Id
                        select user;

            }
            return users.ToList();
        }

        public ApplicationUser GetUser(long userId)
        {
            return _context.Users.Single(x => x.Id == userId);
        }
        public bool ValidateEmail(string email, long tenantId, long? userid)
        {
            var users = _context.Users.Where(x => x.TenantId == tenantId && x.Email == email);
            if (userid.HasValue)
            {
                users = users.Where(x => x.Id != userid.Value);
            }
            return !users.Any();
        }
        public void SaveSystemAdmin(SystemAdminProfileModel systemAdminProfileModel, Stream fileStream, string fileName)
        {
            var user = GetUser(systemAdminProfileModel.UserId);
            user.FirstName = systemAdminProfileModel.FirstName;
            user.LastName = systemAdminProfileModel.LastName;
            user.PhoneNumber = _maskService.RemovePhoneMasking(systemAdminProfileModel.PhoneNumber);
            user.Country = systemAdminProfileModel.Country;
            user.City = systemAdminProfileModel.City;
            user.State = systemAdminProfileModel.State;
            user.Zip = systemAdminProfileModel.Zip;
            string oldImgFile = user.ProfileImage;
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
                user.ProfileImage = _fileService.SaveFile(fileStream, fileName);
            }
            _context.SaveChanges();
            if (fileStream != null && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(oldImgFile))
                _fileService.RemoveFile(oldImgFile);
        }

        public string GetUserProfileImage(long userid)
        {
            return GetUserProfileImage(_context.Users.FirstOrDefault(x => x.Id == userid));
        }

        public string GetUserProfileImage(ApplicationUser user)
        {
            return string.IsNullOrEmpty(user?.ProfileImage) ? _configuration[JMSSetting.DefaultAvtar] : _fileService.GetFile(user.ProfileImage);
        }
    }
}
