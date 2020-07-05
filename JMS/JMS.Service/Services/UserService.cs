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
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;

namespace JMS.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly IMaskService _maskService;
        public UserService(ApplicationDbContext context, IFileService fileService, IConfiguration configuration, IMaskService maskService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _maskService = maskService;
            _userManager = userManager;
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
            return _context.Users.SingleOrDefault(x => x.Id == userId);
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

        public async Task<UserGridModel> GetJournalUsers(string tenantId, UserGridSearchModel userGridSearchModel)
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
            var users = _context.Users.Where(x => x.Tenant.JournalPath == tenantId && (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(userGridSearchModel.Name)
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
        public async Task CreateUser(string tenantId, CreateUserViewModel createUserViewModel)
        {
            var dbTenantId = _context.Tenants.First(x => x.JournalPath == tenantId).Id;
            var user = new ApplicationUser
            {
                FirstName = createUserViewModel.FirstName,
                TenantId = dbTenantId,
                LastName = createUserViewModel.LastName,
                Email = createUserViewModel.Email,
                UserName = createUserViewModel.Email,
                PhoneNumber = createUserViewModel.PhoneNumber,
                IsDisabled = !createUserViewModel.IsActive
            };
            var selectedRoles = createUserViewModel.Roles.Select(x => ((Role)x).ToString());
            var roles = await _context.Roles.Where(x => selectedRoles.Contains(x.Name)).ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        foreach (var role in roles)
                        {
                            _context.UserRoles.Add(new IdentityUserRole<long> { RoleId = role.Id, UserId = user.Id });
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else
                        throw new Exception(result.Errors.Join(","));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task SaveUser(string path, EditUserViewModel editUserViewModel)
        {
            var newroles = editUserViewModel.Roles.Select(x => ((Role)x).ToString());
            var dbRoles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            var journalAdminRole = dbRoles.SingleOrDefault(x => x.Value == Role.JournalAdmin.ToString());
            var userRoles = _context.UserRoles.Where(x => x.UserId == editUserViewModel.UserId && x.RoleId != journalAdminRole.Key).ToArray();
            var journalAdminRoleId = dbRoles.First(x => x.Value == Role.JournalAdmin.ToString()).Key;
            if (userRoles.Any(x => x.RoleId == journalAdminRoleId) && !newroles.Contains(Role.Admin.ToString()))
            {
                throw new Exception("Can not remove Admin role from this user.");
            }
            var user = _context.Users.First(x => x.Tenant.JournalPath == path && x.Id == editUserViewModel.UserId);
            user.FirstName = editUserViewModel.FirstName;
            user.LastName = editUserViewModel.LastName;
            user.IsDisabled = !editUserViewModel.IsActive;
            var deletedRoles = userRoles.Where(x => !newroles.Contains(dbRoles.Single(y => y.Key == x.RoleId).Value)).ToArray();
            var newRoleIds = dbRoles.Where(x => newroles.Contains(x.Value) && !userRoles.Any(y=>y.RoleId==x.Key)).Select(x=>x.Key).ToList();
            _context.UserRoles.RemoveRange(deletedRoles);
            newRoleIds.ForEach(x => _context.UserRoles.Add(new IdentityUserRole<long> { UserId = editUserViewModel.UserId, RoleId = x }));
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();
                }
                catch (Exception)
                {
                    await trans.RollbackAsync();
                    throw;
                }
            }

        }

        private Exception ValidationException()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUser(string path, long userId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId && x.Tenant.JournalPath == path);
            var userRoles = _context.UserRoles.Where(x => x.UserId == userId);
            _context.UserRoles.RemoveRange(userRoles);
            _context.Users.Remove(user);
            using (var tr=_context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await tr.CommitAsync();
                }
                catch (Exception)
                {
                    await tr.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
