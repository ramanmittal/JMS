using JMS.Entity.Data;
using JMS.Service.ServiceContracts;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using JMS.Entity.Entities;
using JMS.ViewModels.Journals;
using System.IO;
using Microsoft.Extensions.Configuration;
using JMS.Service.Settings;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JMS.Service.Enums;
using System.Threading.Tasks;

namespace JMS.Service.Services
{
    public class TenantService : ITenantService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        public readonly IConfiguration _configuration;
        public TenantService(ApplicationDbContext applicationDbContext, IFileService fileService, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _fileService = fileService;
            _configuration = configuration;
            _userManager = userManager;
        }
        public IEnumerable<string> GetTenantPaths()
        {
            return _applicationDbContext.Tenants.IgnoreQueryFilters().Select(x => x.JournalPath).ToList();
        }
        public IEnumerable<Tenant> GetTenants(int? pageIndex, int? pagesize)
        {
            var list = new List<Tenant>();
            if (pageIndex.HasValue && pagesize.HasValue)
            {
                list = _applicationDbContext.Tenants.Skip((pageIndex.Value - 1) * pagesize.Value).Take(pagesize.Value).ToList();
            }
            else
            {
                list = _applicationDbContext.Tenants.ToList();
            }
            return list;
        }
        public long GetTenantsCount()
        {
            return _applicationDbContext.Tenants.Count();
        }

        public async Task CreateTenant(CreateJournalModel model, Stream stream, string journalLogo)
        {
            var roleId = _applicationDbContext.Roles.Single(x => x.Name == Role.Admin.ToString()).Id;
            using (var transaction = _applicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    var tenant = new Tenant { JournalName = model.JournalName, JournalPath = model.JournalPath, JournalTitle = model.JournalTitle, IsDisabled = !model.IsActive };
                    _applicationDbContext.Tenants.Add(tenant);
                    var path = _fileService.SaveFile(stream, journalLogo);
                    tenant.JournalLogo = path;
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, FirstName = model.FirstName, LastName = model.LastName, Tenant = tenant };
                    await _userManager.CreateAsync(user);
                    _applicationDbContext.UserRoles.Add(new IdentityUserRole<long> { RoleId = roleId, UserId = user.Id });
                    _applicationDbContext.JournalAdmins.Add(new JournalAdmin { ApplicationUser = user, Tenant = tenant });
                    _applicationDbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public Tenant GetTenant(long id, params Expression<Func<Tenant, object>>[] includes)
        {
            return _applicationDbContext.Tenants.IncludeMultiple(includes).SingleOrDefault(x => x.Id == id);
        }
        public bool ValidateTenantPath(string JournalPath)
        {
            var paths = (List<string>)GetTenantPaths();
            paths.Add(_configuration[JMSSetting.DefaultTenant]);
            return !paths.Contains(JournalPath, StringComparer.OrdinalIgnoreCase);
        }
        public bool ValidateTenantPath(string JournalPath, long? JournalId)
        {
            if (JournalId.HasValue)
            {
                var paths = _applicationDbContext.Tenants.IgnoreQueryFilters().Where(x => x.Id != JournalId.Value).Select(x => x.JournalPath).ToList();
                paths.Add(_configuration[JMSSetting.DefaultTenant]);
                return !paths.Contains(JournalPath, StringComparer.OrdinalIgnoreCase);
            }
            return ValidateTenantPath(JournalPath);
        }
        public void EditTenant(EditJournalModel model, Stream stream, string fileName)
        {
            var tenant = _applicationDbContext.Tenants.Single(x => x.Id == model.JournalId);
            tenant.IsDisabled = !model.IsActive;
            tenant.JournalName = model.JournalName;
            tenant.JournalTitle = model.JournalTitle;
            if (stream != null && !string.IsNullOrEmpty(fileName))
            {
                _fileService.RemoveFile(tenant.JournalLogo);
                tenant.JournalLogo = _fileService.SaveFile(stream, fileName);
            }
            _applicationDbContext.SaveChanges();
        }
        public void DeleteTenant(long id)
        {
            var deleted = _applicationDbContext.Tenants.Single(x => x.Id==id);
            _applicationDbContext.Tenants.Remove(deleted);
            _applicationDbContext.SaveChanges();
        }

        public void SaveTenantAdmin(EditJournalAdminModel editJournalAdminModel)
        {
            var user=_applicationDbContext.Users.Single(x => x.Id == editJournalAdminModel.UserId);
            user.FirstName = editJournalAdminModel.FirstName;
            user.LastName = editJournalAdminModel.LastName;
            user.PhoneNumber = editJournalAdminModel.PhoneNumber;
            user.IsDisabled = !editJournalAdminModel.Active;
            _applicationDbContext.SaveChanges();
        }

        public async Task CreateTenantAdmin(CreateJournalAdminModel model)
        {
            var roleId=_applicationDbContext.Roles.Single(x => x.Name == Role.Admin.ToString()).Id;
            using (var transaction = _applicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, FirstName = model.FirstName, LastName = model.LastName, TenantId = model.TenantId, IsDisabled = !model.Active };
                    await _userManager.CreateAsync(user);
                    _applicationDbContext.UserRoles.Add(new IdentityUserRole<long> { RoleId = roleId, UserId = user.Id });
                    _applicationDbContext.JournalAdmins.Add(new JournalAdmin { ApplicationUser = user, TenantId = model.TenantId });
                    _applicationDbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task DeleteTenantAdmin(long userId)
        {
            var user = _applicationDbContext.Users.Include(x=>x.JournalAdmin).Single(x => x.Id == userId);
            var journalAdmins = user.JournalAdmin;
            using (var transaction = _applicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    _applicationDbContext.JournalAdmins.Remove(journalAdmins);
                    await _userManager.DeleteAsync(user);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
                
        }
    }
}
