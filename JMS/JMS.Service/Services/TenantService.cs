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
using JMS.ViewModels.Admin;

namespace JMS.Service.Services
{
    public class TenantService : ITenantService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IMaskService _maskService;
        private readonly ICacheService _cahceService;
        public readonly IConfiguration _configuration;
        public TenantService(ApplicationDbContext applicationDbContext, IFileService fileService, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMaskService maskService, ICacheService cahceService)
        {
            _applicationDbContext = applicationDbContext;
            _fileService = fileService;
            _configuration = configuration;
            _userManager = userManager;
            _maskService = maskService;
            _cahceService = cahceService;
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
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = _maskService.RemovePhoneMasking(model.PhoneNumber), FirstName = model.FirstName, LastName = model.LastName, Tenant = tenant };
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
            var oldfile = tenant.JournalLogo;
            if (stream != null && !string.IsNullOrEmpty(fileName))
            {                
                tenant.JournalLogo = _fileService.SaveFile(stream, fileName);
            }
            _applicationDbContext.SaveChanges();
            if (stream != null && !string.IsNullOrEmpty(fileName))
                _fileService.RemoveFile(oldfile);
            _cahceService.ClearJournalCache(tenant.JournalPath);
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
            user.PhoneNumber = _maskService.RemovePhoneMasking(editJournalAdminModel.PhoneNumber);
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
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = _maskService.RemovePhoneMasking(model.PhoneNumber), FirstName = model.FirstName, LastName = model.LastName, TenantId = model.TenantId, IsDisabled = !model.Active };
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

        public Tenant GetTenantByUserId(long userId)
        {
            return _applicationDbContext.Users.Include(x=>x.Tenant).Where(x=>x.Id==userId).Select(x=>x.Tenant).First();
        }

        public void SaveTenant(AdminJournalSettingModel model, Stream stream, string fileName)
        {
            var tenant = _applicationDbContext.Users.Where(x => x.Id == model.UserId).Select(x => x.Tenant).First();
            tenant.IsDisabled = !model.IsActive;
            tenant.JournalName = model.JournalName;
            tenant.JournalTitle = model.JournalTitle;
            tenant.Publisher = model.Publisher;
            tenant.OnlineISSN = model.OnlneISSNNumber;
            tenant.PrintISSN = model.PrintISSNNumber;
            var oldfile = tenant.JournalLogo;
            if (stream != null && !string.IsNullOrEmpty(fileName))
            {
                tenant.JournalLogo = _fileService.SaveFile(stream, fileName);
            }
            _applicationDbContext.SaveChanges();
            if (stream != null && !string.IsNullOrEmpty(fileName))
                _fileService.RemoveFile(oldfile);
            _cahceService.ClearJournalCache(tenant.JournalPath);
        }

        public void SaveTenantContactSetting(JournalContactSettingModel model,long userId)
        {
            var tenant = _applicationDbContext.Users.Where(x => x.Id == userId).Select(x => x.Tenant).First();
            tenant.FirstLine = model.Address;
            tenant.Country = model.Country;
            tenant.City = model.City;
            tenant.State = model.State;
            tenant.Zip = model.Zip;
            tenant.Email = model.Email;
            tenant.PhoneNumber = model.Phone;
            _applicationDbContext.SaveChanges();
        }

        public string AboutUsContent(string tenantPath)
        {
            return _applicationDbContext.JournalSettings.Where(x => x.Tenant.JournalPath == tenantPath && x.Key == JMSSetting.AboutUsContent).Select(x => x.Value).FirstOrDefault();
        }
        public string FooterContent(string tenantPath)
        {
            return _applicationDbContext.JournalSettings.Where(x => x.Tenant.JournalPath == tenantPath && x.Key == JMSSetting.FooterContent).Select(x => x.Value).FirstOrDefault();
        }
        public string AdditionalContent(string tenantPath)
        {
            return _applicationDbContext.JournalSettings.Where(x => x.Tenant.JournalPath == tenantPath && x.Key == JMSSetting.AdditionalContent).Select(x => x.Value).FirstOrDefault();
        }
        public string PrivacyPolicyContent(string tenantPath)
        {
            return _applicationDbContext.JournalSettings.Where(x => x.Tenant.JournalPath == tenantPath && x.Key == JMSSetting.PrivacyPolicyContent).Select(x => x.Value).FirstOrDefault();
        }

        public void SaveAppearanceSettings(AppearanceSettingsModel model, long userId)
        {
            var tenantId = GetTenantByUserId(userId).Id;
            var setting = _applicationDbContext.JournalSettings.Where(x => x.TenantId == tenantId && x.Key == JMSSetting.AboutUsContent).FirstOrDefault();
            if (setting == null)
            {
                setting = new JournalSetting();
                setting.Key = JMSSetting.AboutUsContent; setting.Value = model.AboutUsContent; setting.TenantId = tenantId;
                _applicationDbContext.JournalSettings.Add(setting);
            }
            else
            {
                setting.Value = model.AboutUsContent;
            }
            setting = _applicationDbContext.JournalSettings.Where(x => x.TenantId == tenantId && x.Key == JMSSetting.AdditionalContent).FirstOrDefault();
            if (setting == null)
            {
                setting = new JournalSetting();
                setting.Key = JMSSetting.AdditionalContent; setting.Value = model.AdditionalContent; setting.TenantId = tenantId;
                _applicationDbContext.JournalSettings.Add(setting);
            }
            else
            {
                setting.Value = model.AdditionalContent;
            }
            setting = _applicationDbContext.JournalSettings.Where(x => x.TenantId == tenantId && x.Key == JMSSetting.FooterContent).FirstOrDefault();
            if (setting == null)
            {
                setting = new JournalSetting();
                setting.Key = JMSSetting.FooterContent; setting.Value = model.FooterContent; setting.TenantId = tenantId;
                _applicationDbContext.JournalSettings.Add(setting);
            }
            else
            {
                setting.Value = model.FooterContent;
            }
            setting = _applicationDbContext.JournalSettings.Where(x => x.TenantId == tenantId && x.Key == JMSSetting.PrivacyPolicyContent).FirstOrDefault();
            if (setting == null)
            {
                setting = new JournalSetting();
                setting.Key = JMSSetting.PrivacyPolicyContent; setting.Value = model.PrivacyPolicyContent; setting.TenantId = tenantId;
                _applicationDbContext.JournalSettings.Add(setting);
            }
            else
            {
                setting.Value = model.PrivacyPolicyContent;
            }
            _applicationDbContext.SaveChanges();
        }

        public JournalContactSettingModel GetContactSettings(string tenantPath)
        {
            var tenant = _applicationDbContext.Tenants.FirstOrDefault(x => x.JournalPath == tenantPath);
            if (tenant == null)
            {
                return null;
            }
            return new JournalContactSettingModel
            {
                Address = tenant.FirstLine,
                City = tenant.City,
                Country = tenant.Country,
                Email = tenant.Email,
                Phone = tenant.PhoneNumber,
                State = tenant.State,
                Zip = tenant.Zip
            };
        }
    }
}
