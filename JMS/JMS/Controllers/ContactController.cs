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
    public class ContactController : BaseController
    {
        private readonly ITenantService _tenantService;
        public ContactController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var journal = _tenantService.GetContactSettings(TenantID);
            return View(journal);
        }
    }
}