using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.SystemAdmin
{
    public class ForgotPasswordModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
    }
}
