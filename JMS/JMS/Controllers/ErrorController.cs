using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace JMS.Controllers
{
    public class ErrorController : Controller
    {
        [Route("jms/Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            if (statusCode==403)
            {
                return RedirectToAction("index", "Home");
            }
            return Content("hi");
        }
    }
}