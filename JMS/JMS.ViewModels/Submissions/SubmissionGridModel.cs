using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class SubmissionGridModel
    {
        public int SubmissionId { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string LastActivityDate { get; set; }
        public string SubmissionDate { get; set; }
        public string Status { get; set; }
    }
}
