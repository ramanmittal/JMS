using JMS.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace JMS.Infra.Sequrity
{
    public class JMSPrincipal : ClaimsPrincipal
    {

        public JMSPrincipal(IPrincipal principal, ApplicationUser applicationUser) : base(principal)
        {
            ApplicationUser = applicationUser;
        }

        public ApplicationUser ApplicationUser { get; }
    }
}
