using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Admin
{
    public class AdminJournalSettingModel
    {
        public long UserId { get; set; }
        public string JournalName { get; set; }
        public string JournalTitle { get; set; }
        public bool IsActive { get; set; }
        public string Publisher { get; set; }
        public string OnlneISSNNumber { get; set; }
        public string PrintISSNNumber { get; set; }
    }
}
