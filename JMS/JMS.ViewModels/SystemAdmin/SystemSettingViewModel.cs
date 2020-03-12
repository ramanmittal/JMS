using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.SystemAdmin
{
    public class SystemSettingViewModel
    {
        public string SystemLogo { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string SystemTitle { get; set; }
    }
}
