using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Service.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.Extensions.Configuration;
using JMS.Service.Settings;
using JMS.Service.ServiceContracts;
using System.Threading.Tasks;
using JMS.ViewModels.SystemAdmin;
using System.IO;

namespace JMS.Service.Services
{
    public class SystemService : ISystemService
    {
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;
        private readonly ICacheService _cacheService;
        public SystemService(RoleManager<IdentityRole<long>> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IConfiguration configuration, IFileService fileService, ICacheService cacheService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _fileService = fileService;
            _cacheService = cacheService;
        }

        public async Task  InitializeSystem()
        {
            using (var transaction = _applicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (Role role in (Role[])Enum.GetValues(typeof(Role)))
                    {
                        if (! await _roleManager.RoleExistsAsync(role.ToString()))
                        {
                            await _roleManager.CreateAsync(new IdentityRole<long>(role.ToString()));
                        }
                    }
                    var SystemUserEmail = _configuration[JMSSetting.SystemUserEmail];
                    if (_applicationDbContext.Users.Count(x => x.TenantId == null && x.UserName == SystemUserEmail) == 0)
                    {
                        var user = new ApplicationUser { UserName = SystemUserEmail, Email = SystemUserEmail };
                        await _userManager.CreateAsync(user);
                        await _userManager.AddToRoleAsync(user, Role.SystemAdmin.ToString());
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            
           
        }

        public SystemSettingViewModel GetSystemSettings()
        {
            var model = new SystemSettingViewModel()
            {
                SystemLogo = _configuration[JMSSetting.SystemLogo],
                SystemTitle = _configuration[JMSSetting.SystemTitle]
            };
            var systemSettings = _applicationDbContext.SystemSettings.ToList();
            var setting = systemSettings.FirstOrDefault(x => x.Key == JMSSetting.SystemLogo);
            if (setting != null)
            {
                model.SystemLogo = _fileService.GetFile(setting.Value);               
            }
            setting = systemSettings.FirstOrDefault(x => x.Key == JMSSetting.SystemTitle);
            if (setting != null)
            {
                model.SystemTitle = setting.Value;
            }
            return model;
        }

        public void SetSystemSetting(SystemSettingViewModel systemSettingViewModel, Stream stream, string fileName)
        {
            var settings = _applicationDbContext.SystemSettings.ToList();
            string oldLogoSetting = null;
            if (stream != null && !string.IsNullOrEmpty(fileName))
            {                
                var logo = _fileService.SaveFile(stream, fileName);
                SystemSetting logoSetting = settings.FirstOrDefault(x => x.Key == JMSSetting.SystemLogo);                
                if (logoSetting == null)
                {
                    logoSetting = new SystemSetting { Key = JMSSetting.SystemLogo };
                    _applicationDbContext.SystemSettings.Add(logoSetting);
                }
                else
                {
                    oldLogoSetting = logoSetting.Value;                    
                }
                logoSetting.Value = logo;
                _cacheService.DeleteValue(JMSSetting.SystemLogo);
            }
            if (!string.IsNullOrEmpty(systemSettingViewModel.SystemTitle))
            {
                var setting = settings.FirstOrDefault(x => x.Key == JMSSetting.SystemTitle);
                if (setting == null)
                {
                    setting = new SystemSetting { Key = JMSSetting.SystemTitle };
                    _applicationDbContext.SystemSettings.Add(setting);
                }
                setting.Value = systemSettingViewModel.SystemTitle;
                _cacheService.DeleteValue(JMSSetting.SystemTitle);
            }
            _applicationDbContext.SaveChanges();
            if (stream != null && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(oldLogoSetting))
            {
                _fileService.RemoveFile(oldLogoSetting);
            }
        }
    }
}
