using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using JMS.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
namespace JMS.Controllers
{
    public class ConfirmPhoneController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string token,long userId)
        {
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            var user = userService.GetUser(userId);
            if (user != null)
            {
                var manager = HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                var result = await manager.ChangePhoneNumberAsync(user, user.PhoneNumber, token);
                if (result.Succeeded)
                {
                    AddSuccessMessage(Messages.SuccessPhoneVerificationMessage);
                    return Ok(new { success = true });
                }
                else
                {
                    return Ok(new { success = false });
                }                
            }
            return BadRequest();
        }
        public async Task<IActionResult> ResendCode(long userId)
        {
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            var user = userService.GetUser(userId);
            if (user != null)
            {
                var manager = HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                var token = await manager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                HttpContext.RequestServices.GetService<ISMSService>().Send(new Service.SMS.SMSDetails
                {
                    Message = $"Your verification code is {token}",
                    PhoneNumber = new List<string> { user.PhoneNumber }
                });
            }
            return Ok();
        }
    }
}