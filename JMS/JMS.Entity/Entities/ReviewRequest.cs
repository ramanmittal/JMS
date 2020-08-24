using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Entity.Entities
{
    public class ReviewRequest
    {
        public long ID { get; set; }
        public long ReviewerID { get; set; }
        public virtual ApplicationUser Reviewer { get; set; }
        public long SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public ReviewType ReviewType { get; set; }
        public string EditorComment { get; set; }
    }
}
