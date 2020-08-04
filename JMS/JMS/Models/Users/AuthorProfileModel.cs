using JMS.Models.SystemAdmin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Users
{
    public class AuthorProfileModel : BaseProfileModel
    {
        [Required]
        [DisplayName("Affiliation")]
        public new string AffiliationNo { get; set; }
        [DisplayName("ORCID ID")]
        [RegularExpression(@"\d{4}-\d{4}-\d{4}-\d{4}", ErrorMessage = "Invalid ORCID")]
        public string ORCID { get; set; }
    }
}
