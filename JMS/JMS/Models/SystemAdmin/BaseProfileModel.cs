using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.SystemAdmin
{
    public class BaseProfileModel
    {
        public string ProfileImagePath { get; set; }
        [DisplayName("Profile Image")]
        public IFormFile ProfileImageFile { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("PhoneNumber")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number.")]
        public string PhoneNumber { get; set; }
        [Required]
        [DisplayName("Country")]
        public string Country { get; set; }
        [Required]
        [DisplayName("City")]
        public string City { get; set; }
        [Required]
        [DisplayName("State")]
        public string State { get; set; }
        [Required]
        [DisplayName("Zip")]
        public string Zip { get; set; }
        public string AffiliationNo { get; set; }
    }
}
