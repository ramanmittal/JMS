using JMS.Service.ServiceContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using JMS.Entity.Entities;
using JMS.Entity.Data;

namespace JMS.Infra.Sequrity
{
    public class JMSAuthenticationService: AuthenticationService
    {
        public JMSAuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers, IClaimsTransformation transform, IOptions<AuthenticationOptions> options):base(schemes, handlers, transform, options)
        {

        }

        public override async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            var result = await base.AuthenticateAsync(context, scheme);
            if (!result.Succeeded)
            {
                return result;
            }
            var userId = long.Parse(result.Principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var userService = context.RequestServices.GetService<IUserService>();
            var user = context.RequestServices.GetService<IUserService>().GetUser(userId);

            if (user != null)
            {
                var roles = userService.GetRoles(userId);
                var roleClaims = result.Principal.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
                var identity = (ClaimsIdentity)result.Principal.Identity;
                foreach (var roleClaim in roleClaims)
                {
                    identity.RemoveClaim(roleClaim);
                }
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
                return AuthenticateResult.Success(new AuthenticationTicket(new JMSPrincipal(result.Principal, user), result.Ticket.AuthenticationScheme));
            }
            var signInManager = (SignInManager<ApplicationUser>)context.RequestServices.GetService(typeof(SignInManager<ApplicationUser>));
            await signInManager.SignOutAsync();
            return AuthenticateResult.Fail(string.Empty);


        }
    }
}
