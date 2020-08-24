using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class RejectedSubmissionGridModel
    {
        public int ItemsCount { get; set; }
        public IEnumerable<RejectedSubmissionGridRowModel> Data { get; set; }
    }
    public class RejectedSubmissionGridSearchModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

        private string srchText;
        public string SrchText
        {
            get { return srchText; }
            set { srchText = value?.Trim(); }
        }
        private string author;
        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public long? EditerId { get; set; }

    }
    public class RejectedSubmissionGridRowModel
    {
        public long SubmissionID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        public string SubTitle { get; set; }
        public string LastUpdated { get; set; }
    }
}
