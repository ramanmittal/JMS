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
        public IEnumerable<JournalAdminModel> JounralAdmins { get; set; }
        public class JournalAdminModel
        {
            public long UserId { get; set; }
            [Required]
            [DisplayName("First Name")]
            public string FirstName { get; set; }
            [DisplayName("Last Name")]
            [Required]
            public string LastName { get; set; }      
            public string Email { get; set; }
            [DisplayName("Phone Number")]
            [Phone]
            [RegularExpression(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$", ErrorMessage = "The Phone Number field is not a valid phone number")]
            [Required]
            public string PhoneNumber { get; set; }
            public bool Active { get; set; }
        }
    }
}
