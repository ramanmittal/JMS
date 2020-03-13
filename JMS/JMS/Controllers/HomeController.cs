using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JMS.Models;
using Microsoft.AspNetCore.Identity;
using JMS.Service.ServiceContracts;
using Microsoft.AspNetCore.Http;
using JMS.Setting;

namespace JMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISystemService _systemService;
        public HomeController(ILogger<HomeController> logger, ISystemService systemService)
        {
            _logger = logger;
            _systemService = systemService;
        }

        public async Task<IActionResult> InitializeJMS()
        {
            await _systemService.InitializeSystem();
            return new ContentResult { StatusCode = StatusCodes.Status200OK, Content = Messages.Intialization };
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (User.IsInRole(RoleName.SystemAdmin))
            {
                return RedirectToAction("Index", "SystemAdmin");
            }
            return Unauthorized();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
