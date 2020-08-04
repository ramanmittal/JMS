using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JMS.Entity.Entities
{
    public class Submission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Prefix { get; set; }
        [Required]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public SubmissionStatus SubmissionStatus { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        public string Keywords { get; set; }
        public int CreateStep { get; set; }
        public ApplicationUser User { get; set; }
        public long UserID { get; set; }
        public string EditorComment { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
