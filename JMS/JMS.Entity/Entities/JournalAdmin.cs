using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JMS.Entity.Entities
{
    public class JournalAdmin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public long UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
