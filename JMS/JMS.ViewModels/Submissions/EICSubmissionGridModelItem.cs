using JMS.Entity.Data;
using JMS.ViewModels.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class EICSubmissionGridModel
    {
        public int ItemsCount { get; set; }
        public IEnumerable<EICSubmissionGridModelItem> Data { get; set; }
    }
    public class EICSubmissionGridModelItem
    {
        public long SubmissionID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        public string SubTitle { get; set; }
        public string Status { get; set; }
        public string LastUpdated { get; set; }
    }
    public class EditorSubmissionGridSearchModel
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
        public SubmissionStatus? SubmissionStatus { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public EditorAssignedStatus AssignedStatus { get; set; }
        public long? EditerId { get; set; }
    }

}
