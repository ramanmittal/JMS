using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Register
{
    public class RegisterAuthorModel
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Remote("ValidateJournalUserEmail", "Account", ErrorMessage = JMS.Setting.Messages.EmailNotAvailiable)]
        public string Email { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number.")]
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Affiliation { get; set; }
        
        [Required]
        public string Country { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "The Password Field must have at least 6 characters, 1 digit, 1 lowercase letter, 1 uppercase letter.")]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }


    }
}
