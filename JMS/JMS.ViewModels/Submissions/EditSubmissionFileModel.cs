using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class EditSubmissionFileModel
    {
        public long SubmissionFileId { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }
        [DisplayName("Article Componenets")]
        public long ArticleComponentId { get; set; }
    }
}
