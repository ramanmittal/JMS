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
using Microsoft.Extensions.DependencyInjection;
using JMS.Service.Settings;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JMS.ViewModels.Users;
using JMS.Service.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using JMS.ViewModels.Register;

namespace JMS.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly IMaskService _maskService;
        private readonly IServiceProvider _serviceProvider;
        public UserService(ApplicationDbContext context, IFileService fileService, IConfiguration configuration, IMaskService maskService, UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _maskService = maskService;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
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
            user.AffiliationNo = systemAdminProfileModel.AffiliationNo;
            string oldImgFile = user.ProfileImage;
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
                user.ProfileImage = _fileService.SaveFile(fileStream, fileName);
            }
            _context.SaveChanges();
            if (fileStream != null && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(oldImgFile))
                _fileService.RemoveFile(oldImgFile);
        }

        public void SaveAuthor(AuthorProfileModel authorProfileModel, Stream fileStream, string fileName) {
            var author = _context.Authors.Single(x => x.Id == authorProfileModel.UserId);
            author.Orcid = authorProfileModel.ORCID;
            using (var tr = _context.Database.BeginTransaction())
            {
                try
                {
                    SaveSystemAdmin(authorProfileModel, fileStream, fileName);
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    throw;
                }                
            }            
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
            var accpetedRoles = _context.Roles.Where(x => x.Name != Role.Author.ToString() && x.Name != Role.JournalAdmin.ToString() && x.Name != Role.SystemAdmin.ToString());
            IQueryable<IdentityRole<long>> dbRoles = roles != null && roles.Any() ? accpetedRoles.Where(x => roles.Contains(x.Name)) : accpetedRoles;
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
                                   join role in accpetedRoles on userRole.RoleId equals role.Id
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
                ItemsCount = await filteredUsers.CountAsync()
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
                PhoneNumber = _maskService.RemovePhoneMasking(createUserViewModel.PhoneNumber),
                IsDisabled = !createUserViewModel.IsActive,
                AffiliationNo = createUserViewModel.AffiliationNo
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
            user.PhoneNumber = _maskService.RemovePhoneMasking(editUserViewModel.PhoneNumber);
            user.IsDisabled = !editUserViewModel.IsActive;
            user.AffiliationNo = editUserViewModel.AffiliationNo;
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

        public async Task<ApplicationUser> CreateAuthor(string path, RegisterAuthorModel model)
        {
            var tenanID = _context.Tenants.Single(x => x.JournalPath == path).Id;
            var applicationUser = new ApplicationUser
            {
                Country = model.Country,
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = _maskService.RemovePhoneMasking(model.PhoneNumber),
                AffiliationNo = model.Affiliation,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TenantId = tenanID
            };
            var authorRoleId = _context.Roles.Single(x => x.Name == Role.Author.ToString()).Id;
            using (var tr = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(applicationUser, model.Password);
                    if (result.Succeeded)
                    {
                        _context.UserRoles.Add(new IdentityUserRole<long> { RoleId = authorRoleId, UserId = applicationUser.Id });
                        _context.Authors.Add(new Author { User = applicationUser });
                        await _context.SaveChangesAsync();
                        await tr.CommitAsync();
                        return applicationUser;
                    }
                    return null;
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public Author GetAuthor(long userId)
        {
            var author = _context.Authors.SingleOrDefault(x => x.Id == userId);
            return author;
        }

        public IDictionary<long,string> GetJounalEditors(string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var editorRoles = context.Roles.Where(x => x.Name == Role.EIC.ToString() || x.Name == Role.SectionEditor.ToString());
            var userIds = (from usrRole in context.UserRoles
                           join
                           role in editorRoles on usrRole.RoleId equals role.Id
                           select usrRole.UserId).Distinct();
            var users = (from user in context.Users.Where(x => x.Tenant.JournalPath == journalPath)
                         join
                         userId in userIds on user.Id equals userId
                         select new { user.Id, user.FirstName, user.LastName }).ToList();
            return users.ToDictionary(x => x.Id, x => $"{ x.FirstName} {x.LastName}");
        }

        public IEnumerable<string> GetRoles(long userId)
        {
            return (from role in _context.Roles
                    join
                    userRole in _context.UserRoles.Where(x => x.UserId == userId) on role.Id equals userRole.RoleId
                    select role.Name).ToArray();
        }

        public int AssignedSubmissionAsEditor(long userId, string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var inactiveStatus = new SubmissionStatus[] { SubmissionStatus.Draft, SubmissionStatus.Rejected };
            var editorCount = context.Submission.Count(x => x.User.Tenant.JournalPath == journalPath && x.EditorId == userId && !inactiveStatus.Contains(x.SubmissionStatus));
            return editorCount;
        }
        public int AssignedSubmissionAsReviewer(long userId, string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var reviewerCount = context.Submission.Count(x => x.User.Tenant.JournalPath == journalPath && x.EditorId == userId && (x.SubmissionStatus== SubmissionStatus.Review));
            return reviewerCount;
        }
    }
}
