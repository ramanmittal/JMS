using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Infra.Sequrity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JMS.Controllers
{
    public class MySubmissionController : BaseController
    {
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult Index()
        {
            return View();
        }
    }
}