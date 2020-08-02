using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JMS.Entity.Entities
{
    public class Contributor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Country { get; set; }
        public string ORCIDiD { get; set; }
        [Required]
        public string AffiliationNo { get; set; }
        [Required]
        public ContributerRole ContributerRole { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
