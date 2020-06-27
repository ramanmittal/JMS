using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using JMS.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
namespace JMS.Controllers
{
    public class UsersController : BaseController
    {
        [Authorize(Roles = RoleName.Admin)]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = RoleName.Admin)]
        public async Task<UserGridModel> JournalUsers([FromQuery]UserGridSearchModel userGridSearchModel)
        {
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            var tenantID = long.Parse(TenantID);
            var users = await userService.GetJournalUsers(tenantID, userGridSearchModel);
            return users;
        }
        [Authorize(Roles = RoleName.Admin)]
        public IActionResult Create()
        {
            return View();
        }
    }
}