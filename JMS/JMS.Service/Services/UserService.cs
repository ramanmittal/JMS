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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JMS.ViewModels.Users;
using JMS.Service.Enums;
using Microsoft.AspNetCore.Identity;

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

        public ApplicationUser GetUserByEmail(string email, string path)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email && x.Tenant.JournalPath == path);
        }

        public async Task<UserGridModel> GetJournalUsers(long tenantId, UserGridSearchModel userGridSearchModel)
        {
            string[] roles = null;
            if (userGridSearchModel.Roles != null)
            {
                roles = userGridSearchModel.Roles.Split(",").Select(x => x.Trim()).ToArray();
                if (roles.Any())
                {
                    roles = roles.Select(x => ((Role)int.Parse(x)).ToString()).ToArray();
                }
            }
            var users = _context.Users.Where(x => x.TenantId == tenantId && (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(userGridSearchModel.Name)
                         && x.Email.Contains(userGridSearchModel.Email));
            if (userGridSearchModel.Status.HasValue)
            {
                users = userGridSearchModel.Status.Value ? users.Where(x => x.IsDisabled != true) : users.Where(x => x.IsDisabled == true);
            }
            IQueryable<IdentityRole<long>> dbRoles = roles != null && roles.Any() ? _context.Roles.Where(x => roles.Contains(x.Name)) : _context.Roles;
            var filteredUsers = (from userRole in _context.UserRoles
                        join user in users on userRole.UserId equals user.Id
                        join role in dbRoles on userRole.RoleId equals role.Id

                        select new { UserId = user.Id, user.FirstName, user.LastName, user.Email, user.ProfileImage } into g
                        group g by g.UserId into g
                        select new
                        {
                            UserId = g.Key,
                            FirstName = g.Max(x => x.FirstName),
                            LastName = g.Max(x => x.LastName),
                            Email = g.Max(x => x.Email),
                            ProfileImage = g.Max(x => x.ProfileImage)
                        });
            if (userGridSearchModel.sortField == "Name" && userGridSearchModel.sortOrder == "asc")
            {
                filteredUsers = filteredUsers.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            }
            else if (userGridSearchModel.sortField == "Name" && userGridSearchModel.sortOrder == "desc")
            {
                filteredUsers = filteredUsers.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName);
            }
            if (userGridSearchModel.sortField == "Email" && userGridSearchModel.sortOrder == "asc")
            {
                filteredUsers = filteredUsers.OrderBy(x => x.Email);
            }
            else if (userGridSearchModel.sortField == "Email" && userGridSearchModel.sortOrder == "desc")
            {
                filteredUsers = filteredUsers.OrderByDescending(x => x.Email);
            }

            var extractedUsers = await filteredUsers.Skip((userGridSearchModel.pageIndex - 1) * userGridSearchModel.pageSize).Take(userGridSearchModel.pageSize).Select(x => new { x.UserId, x.FirstName, x.LastName, x.Email, x.ProfileImage }).ToArrayAsync();
            var userIds = extractedUsers.Select(x => x.UserId);
            var userRoles = await (from userRole in _context.UserRoles
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 join user in _context.Users on userRole.UserId equals user.Id
                                 where userIds.Contains(userRole.UserId)
                                 select new { UserId = userRole.UserId, Role = role.Name, user.IsDisabled }
                        ).ToArrayAsync();
            var groupedUsers = userRoles.GroupBy(x => x.UserId);
            var data = new UserGridRowModel[extractedUsers.Length];
            for (int i = 0; i < extractedUsers.Length; i++)
            {
                var currentUser = extractedUsers[i];
                var groupedUser = groupedUsers.First(z => z.Key == currentUser.UserId);
                data[i] = new UserGridRowModel
                {
                    ProfileImage = string.IsNullOrEmpty(currentUser.ProfileImage) ? _configuration[JMSSetting.DefaultAvtar] : _fileService.GetFile(currentUser.ProfileImage),
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    IsDisabled = groupedUser.First().IsDisabled,
                    UserId = currentUser.UserId,
                    UserName = currentUser.Email,
                    Roles = groupedUser.Select(x => x.Role).ToArray()
                };
            }
            return new UserGridModel
            {
                Data = data,
                TotalItems = await filteredUsers.CountAsync()
            };
        }
    }
}
