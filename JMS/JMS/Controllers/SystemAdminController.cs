using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.Enums;
using JMS.Setting;
using JMS.ViewModels.SystemAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.SystemAdmin)]
    public class SystemAdminController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel  model)
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForGotPassword()
        {
            return View();
        }
    }
}