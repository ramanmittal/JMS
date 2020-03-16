using JMS.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JMS.Service.ServiceContracts
{
    public interface IAccountService
    {
        Task<string> GetResetPasswordTokenByEmail(string email,string tenantId);
        Task<bool> VerifyUserTokenAsync(string email, string token,string tenantId);
        Task<IdentityResult> ResetPassword(string email, string token, string password,string tenantId);
        Task<IdentityResult> ChangePassword(long userId, string password, string confirmPassword);
    }
}
