using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Infra.Sequrity
{
    public class PreventDisableUserHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var isUserDisabled = ((JMSPrincipal)context.User).ApplicationUser.IsDisabled;
                if (isUserDisabled != true)
                {
                    if (requirement.AllowedRoles.All(x => context.User.IsInRole(x)))
                        context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                } 
            }
        }
    }
}
