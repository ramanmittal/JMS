using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.SystemAdmin
{
    public class ChangePassword
    {
        [Required]
        public string Password { get; set; }
        [DisplayName("New Password")]
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
