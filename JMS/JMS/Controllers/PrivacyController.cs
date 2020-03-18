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
    public class PrivacyController : BaseController
    {
        private readonly ITenantService _tenantService;
        public PrivacyController(ITenantService tenantService, IConfiguration configuration) : base(configuration)
        {
            _tenantService = tenantService;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var privacyPolicyContent = _tenantService.PrivacyPolicyContent(TenantID);
            return View(model: privacyPolicyContent);
        }
    }
}