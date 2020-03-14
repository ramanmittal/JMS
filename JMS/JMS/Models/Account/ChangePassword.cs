using JMS.Helpers.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.Models.Account
{
    public class ChangePassword
    {
        [Required]
        public string Password { get; set; }
        [DisplayName("New Password")]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",ErrorMessage = "The New Password Field must have at least 6 characters, 1 digit, 1 lowercase letter, 1 uppercase letter.")]
        public string NewPassword { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
