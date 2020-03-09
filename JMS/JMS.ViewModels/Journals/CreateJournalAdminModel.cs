using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Journals
{
    public class CreateJournalAdminModel
    {
        public long TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
    }
}
