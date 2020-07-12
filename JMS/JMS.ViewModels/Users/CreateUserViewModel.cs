using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Users
{
    public class CreateUserViewModel
    {
       
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
             
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public string AffiliationNo { get; set; }
        public List<int> Roles { get; set; }
    }
}
