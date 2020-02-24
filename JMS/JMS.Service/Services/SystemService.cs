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

namespace JMS.Service.Services
{
    public class SystemService : ISystemService
    {
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        public SystemService(RoleManager<IdentityRole<long>> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
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
    }
}
