using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.SystemAdmin)]
    public class AccountController : BaseController
    {
        private readonly IUserService _userservice;
        
        public AccountController(IConfiguration configuration, IUserService userservice) :base(configuration)
        {
            _userservice = userservice;
        }
        [AllowAnonymous]
        public IActionResult ValidateEmail(string email, long tenantId, long? userid)
        {
            if (!_userservice.ValidateEmail(email, tenantId, userid))
            {
                return Json(data: Messages.EmailNotAvailiable);
            }
            return Json(data: true);
        }
    }
}