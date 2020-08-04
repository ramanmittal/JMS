using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class SubmissionGridModel
    {
        public int ItemsCount { get; set; }
        public IEnumerable<SubmissionGridRowModel> Data { get; set; }
    }
    public class SubmissionGridSearchModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value?.Trim(); }
        }
        private string keywords;

        public string Keywords
        {
            get { return keywords; }
            set { keywords = value?.Trim(); }
        }
        public SubmissionStatus? Status { get ; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }

    }
    public class SubmissionGridRowModel
    {
        public long SubmissionId { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string LastActivityDate { get; set; }
        public string SubmissionDate { get; set; }
        public string Status { get; set; }
    }
}
