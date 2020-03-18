using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Service.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    public class AboutController : BaseController
    {
        private readonly ITenantService _tenantService;
        public AboutController(ITenantService tenantService, IConfiguration configuration) : base(configuration)
        {
            _tenantService = tenantService;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var aboutUsContent = _tenantService.AboutUsContent(TenantID);
            return View(model: aboutUsContent);
        }
    }
}