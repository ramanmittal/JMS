using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JMS.Service.ServiceContracts
{
    public interface IAccountService
    {
        Task<string> GetResetPasswordTokenByEmail(string email);
    }
}
