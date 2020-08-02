using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class AddContributerViewModel
    {
        public long SubmissionId { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Country { get; set; }
        [DisplayName("ORCID ID")]
        public string ORCIDiD { get; set; }
        [Required]
        [DisplayName("Affiliation")]
        public string AffiliationNo { get; set; }
        [Required]
        [DisplayName("Role")]
        public ContributerRole ContributerRole { get; set; }
    }
}
