using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Journals
{
    public class CreateJournalAdminModel
    {
        public long TenantId { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Remote("ValidateEmail", "Account", AdditionalFields = "TenantId",ErrorMessage = JMS.Setting.Messages.EmailNotAvailiable)]
        public string Email { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        [RegularExpression(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$", ErrorMessage = "The Phone Number field is not a valid phone number")]
        [Required]
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
    }
}
