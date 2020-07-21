using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
namespace JMS.Controllers
{
    public class ConfirmEmailController : BaseController
    {
        public async Task<IActionResult> Index(string token, long userId)
        {
            var user = HttpContext.RequestServices.GetService<IUserService>().GetUser(userId);
            if (!user.EmailConfirmed)
            {
                var userManager = HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                if (await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, UserManager<ApplicationUser>.ConfirmEmailTokenPurpose, token))
                {
                    await userManager.ConfirmEmailAsync(user, token);
                    var otp = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                    HttpContext.RequestServices.GetService<ISMSService>().Send(new Service.SMS.SMSDetails
                    {
                        Message = $"Your verification code is {otp}",
                        PhoneNumber = new List<string> { user.PhoneNumber }
                    });
                    return View(userId);
                }
                return View("linkexpiration");
            }
            return View("emailvarified");
        }
    }
}