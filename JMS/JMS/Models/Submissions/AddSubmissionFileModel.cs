using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JMS.Models.Submissions
{
    public class AddSubmissionFileModel
    {
        public long SubmissionId { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }
        public long ArticleComponentId { get; set; }
        [Required]
        public IFormFile File { get; set; }

    }
}