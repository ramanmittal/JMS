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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsDisabled { get; set; }
        public virtual JournalAdmin JournalAdmin { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ProfileImage { get; set; }
    }
}
