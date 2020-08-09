using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class AssignedSubmissionCount
    {
        public int Assigned { get; set; }
        public int UnAssigned { get; set; }
        public IDictionary<long, string> Editors { get; set; }
    }
}
