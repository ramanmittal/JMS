using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JMS.Entity.Entities
{
    public class SubmisssionFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string FileId { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public DateTime UploadedOn { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }
        public long SubmissionId { get; set; }
        public Submission Submission { get; set; }
        public long ArticleComponentId { get; set; }
        public virtual TenantArticleComponent TenantArticleComponent { get; set; }
        
    }
}