using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class ActivityLog
    {
        public string LoggedDate { get; set; }
        public string Action { get; set; }
        public string ActorName { get; set; }
        public string ActorEmail { get; set; }
    }
}
