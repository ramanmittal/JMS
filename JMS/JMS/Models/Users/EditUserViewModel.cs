using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Users
{
    public class EditUserViewModel
    {
        public long UserId { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number.")]
        [Required]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<SelectListItem> AssignedRoles { get; set; }
        public string AffiliationNo { get; set; }
        [Required]
        public List<int> Roles { get; set; }
        public bool IsJournalAdmin { get; set; }
        public int ReviewCount { get; internal set; }
        public int EditorCount { get; internal set; }
    }
}
