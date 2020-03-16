using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using JMS.ViewModels.SystemAdmin;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;

namespace JMS.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AccountService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<string> GetResetPasswordTokenByEmail(string email, string tenantId)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email && x.Tenant.JournalPath == tenantId);
            if (user == null)
            {
                return null;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }
        public async Task<bool> VerifyUserTokenAsync(string email,string token, string tenantId)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email && x.Tenant.JournalPath == tenantId);
            if (user!=null)
            {
              return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
            }
            return false;
        }

        public async Task<IdentityResult> ResetPassword(string email, string token, string password, string tenantId)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email && x.Tenant.JournalPath == tenantId);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, password);
                return result;
            }
            return null;
        }

        public async Task<IdentityResult> ChangePassword(long userId,string password,string confirmPassword)
        {
            var result = await _userManager.ChangePasswordAsync(await _userManager.FindByIdAsync(userId.ToString()), password, confirmPassword);
            return result;
        }
    }
}
