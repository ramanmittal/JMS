using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.SystemAdmin
{
    public class SystemSettingViewModel
    {
        public string SystemLogo { get; set; }
        [DisplayName("System Logo")]
        public IFormFile SystemLogoFile { get; set; }
        [Required]
        [MaxLength(10)]
        [DisplayName("System Title")]
        public string SystemTitle { get; set; }
    }
}
