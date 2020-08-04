using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.Entity.Entities
{
    public class Author
    {
        [Key]
        public long Id { get; set; }
        
        public string Orcid { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
