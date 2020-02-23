using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Entity.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public long? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public virtual JournalAdmin JournalAdmin { get;set;}
}
}
