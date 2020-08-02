using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JMS.Entity.Entities
{
    public class TenantArticleComponent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
        public long? TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
