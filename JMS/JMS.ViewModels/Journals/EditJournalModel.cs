using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Journals
{
    public class EditJournalModel
    {
        public long JournalId { get; set; }

        public string JournalName { get; set; }
        
        public string JournalTitle { get; set; }
        
        //public string JournalPath { get; set; }
        public bool IsActive { get; set; }
    }
}
