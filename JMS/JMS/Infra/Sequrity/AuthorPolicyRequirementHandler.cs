using JMS.Service.Enums;
using JMS.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JMS.Infra.Sequrity
{
    public static class HttpContextExtensions
    {
        private static readonly RouteData EmptyRouteData = new RouteData();

        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();

        public static Task ExecuteResultAsync<TResult>(this HttpContext context, TResult result)
            where TResult : IActionResult
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (result == null) throw new ArgumentNullException(nameof(result));

            var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<TResult>>();

            var routeData = context.GetRouteData() ?? EmptyRouteData;
            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

            return executor.ExecuteAsync(actionContext, result);
        }
    }
    public class AuthorPolicyRequirement: IAuthorizationRequirement
    {

    }
    public class AuthorPolicyRequirementHandler : AuthorizationHandler<AuthorPolicyRequirement> 
    {
        public const string AuthorPolicy = "AuthorPolicy";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorPolicyRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorPolicyRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var jmsPrincipal = ((JMSPrincipal)context.User);
                if (jmsPrincipal.IsInRole(Role.Author.ToString()))
                {
                    if (jmsPrincipal.ApplicationUser.IsDisabled.GetValueOrDefault())
                    {
                        context.Fail();
                        return;
                    }
                    context.Succeed(requirement);
                    if (!jmsPrincipal.ApplicationUser.EmailConfirmed)
                    {
                        await _httpContextAccessor.HttpContext.ExecuteResultAsync(
                         new RedirectToActionResult("VerifyEmail", "Account", null));
                        return;
                    }
                    if (!jmsPrincipal.ApplicationUser.PhoneNumberConfirmed)
                    {
                        await _httpContextAccessor.HttpContext.ExecuteResultAsync(
                         new RedirectToActionResult("VerifyPhone", "Account", null));
                        return;
                    }
                    if (!context.User.IsInRole(RoleName.AuthorizedAuthor))
                    {
                        ((ClaimsIdentity)context.User.Identity).AddClaim(new Claim(ClaimTypes.Role, RoleName.AuthorizedAuthor));
                    }
                    context.Succeed(requirement);
                    return;
                }
            }
            context.Fail();
        }
    }
}
