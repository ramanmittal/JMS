using JMS.Entity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Submissions
{
    public class EditContributerModel
    {
        public long ContributerId { get; set; }
        public long SubmissionId { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [Remote("ValidateContributerEmail", "Submission", AdditionalFields = "SubmissionId,ContributerId", ErrorMessage = "A contribiuter having this email already exist.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Country { get; set; }
        [DisplayName("ORCID")]
        [RegularExpression(@"\d{4}-\d{4}-\d{4}-\d{4}", ErrorMessage = "Invalid ORCID")]
        public string ORCIDiD { get; set; }
        [Required]
        [DisplayName("Affiliation")]
        public string AffiliationNo { get; set; }
        [Required]
        [DisplayName("Role")]
        public ContributerRole ContributerRole { get; set; }
    }
}
