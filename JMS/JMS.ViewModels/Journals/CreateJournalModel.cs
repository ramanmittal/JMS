using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Journals
{
    public class CreateJournalModel
    {
        [Required]
        [DisplayName("Journal Name")]
        public string JournalName { get; set; }
        [Required]
        [DisplayName("Journal Title")]
        [MaxLength(20)]
        public string JournalTitle { get; set; }
        [Required]
        [DisplayName("Journal Path")]
        [MaxLength(10)]
        public string JournalPath { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
