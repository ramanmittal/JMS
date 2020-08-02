using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class EditSubmissionMetadataModel
    {
        public long Id { get; set; }
        public string Prefix { get; set; }
        [Required]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        public string Keywords { get; set; }
    }
}
