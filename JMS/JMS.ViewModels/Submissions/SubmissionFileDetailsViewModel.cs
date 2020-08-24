using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class SubmissionFileDetailsViewModel
    {
        public string FileName { get; set; }
        public string UploadedOn { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }
        public string ArticleComponent { get; set; }
    }
}
