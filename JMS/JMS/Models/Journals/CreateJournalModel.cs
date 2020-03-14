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
    public class CreateJournalModel
    {
        [Required]
        [DisplayName("Journal Name")]
        public string JournalName { get; set; }
        [Required]
        [DisplayName("Journal Title")]
        [MaxLength(20)]
        public string JournalTitle { get; set; }
        [Required]
        [DisplayName("Journal Path")]
        [MaxLength(10)]
        [Remote("ValidateTenantPath", "Tenant")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = Messages.AlphaNumericValidationMessage)]
        public string JournalPath { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number.")]
        [Required]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public IFormFile Logo { get; set; }
    }
}
