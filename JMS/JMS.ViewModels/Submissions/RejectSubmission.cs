using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class RejectSubmission
    {
        [Required]
        public long Id { get; set; }
        public string RejectComment { get; set; }
    }
}
