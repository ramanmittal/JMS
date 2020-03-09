using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Journals
{
    public class EditJournalAdminModel
    {
        public long UserId { get; set; }
        
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
       
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
    }
}
