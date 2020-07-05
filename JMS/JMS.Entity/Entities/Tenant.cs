using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JMS.Entity.Entities
{
    public class Tenant : Trackable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string JournalName { get; set; }
        [Required]
        public string JournalTitle { get; set; }
        [Required]
        public string JournalLogo { get; set; }
        [Required]
        public string JournalPath { get; set; }
        public bool? IsDisabled { get; set; }
        public string Publisher { get; set; }
        public string OnlineISSN { get; set; }
        public string PrintISSN { get; set; }
        public string FirstLine { get; set; }
        public string LastLine { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public virtual ICollection<JournalSetting> JournalSettings { get; set; }

    }
}
