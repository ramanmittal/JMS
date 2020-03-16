using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Admin
{
    public class AdminJournalSettingModel
    {
        [Required]
        [DisplayName("Journal Name")]
        public string JournalName { get; set; }
        [Required]
        [DisplayName("Journal Title")]
        [MaxLength(20)]
        public string JournalTitle { get; set; }
        public bool IsActive { get; set; }
        public string LogoPath { get; set; }
        public IFormFile Logo { get; set; }
        public string Publisher { get; set; }
        [DisplayName("Onlne ISSN")]
        [RegularExpression("[0-9][0-9][0-9][0-9][-][0-9][0-9][0-9][X0-9]",ErrorMessage ="Invalid ISSN.")]
        public string OnlneISSNNumber { get; set; }
        [DisplayName("Print ISSN")]
        [RegularExpression("[0-9][0-9][0-9][0-9][-][0-9][0-9][0-9][X0-9]", ErrorMessage = "Invalid ISSN.")]
        public string PrintISSNNumber { get; set; }
    }
}
