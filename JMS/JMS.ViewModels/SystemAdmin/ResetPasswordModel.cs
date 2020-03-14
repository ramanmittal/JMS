using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.SystemAdmin
{
    public class ResetPasswordModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "The Password Field must have at least 6 characters, 1 digit, 1 lowercase letter, 1 uppercase letter.")]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
