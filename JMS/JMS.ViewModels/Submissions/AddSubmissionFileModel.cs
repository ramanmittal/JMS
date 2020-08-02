using System.IO;

namespace JMS.ViewModels.Submissions
{
    public class AddSubmissionFileModel
    {
        public long SubmissionId { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }
        public long ArticleComponentId { get; set; }

        public Stream FileStream { get; set; }
        public string FileName { get; set; }

    }
}