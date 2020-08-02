using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JMS.Models.Submissions
{
    public class AddSubmissionFileViewModel
    {
        public long SubmissionId { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }        
        public IEnumerable<SelectListItem> ArticleComponenets { get; set; }
        [Display(Name="Article Componenets")]
        public long ArticleComponentId { get; set; }
        [Required]
        public IFormFile File { get; set; }

    }
}