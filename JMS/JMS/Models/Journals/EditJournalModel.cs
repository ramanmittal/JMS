using JMS.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Journals
{
    public class EditJournalModel
    {
        [Required]
        public long JournalId { get; set; }
        [Required]
        [DisplayName("Journal Name")]
        public string JournalName { get; set; }
        [Required]
        [DisplayName("Journal Title")]
        [MaxLength(20)]
        public string JournalTitle { get; set; }
        //[Required]
        //[DisplayName("Journal Path")]
        //[MaxLength(10)]
        //[Remote("ValidateTenantPath", "Tenant", AdditionalFields= "JournalId")]
        //[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = Messages.AlphaNumericValidationMessage)]
        //public string JournalPath { get; set; }
        public bool IsActive { get; set; }       
        public IFormFile Logo { get; set; }        
        public string LogoPath { get; set; }
    }
}
