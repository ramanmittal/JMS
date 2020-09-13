using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.EmailModels
{
    public class AddUserEmailNotificationModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JournalName { get; set; }
        public List<string> RoleNames { get; set; }
        public string JournalPath { get; set; }
    }
}
