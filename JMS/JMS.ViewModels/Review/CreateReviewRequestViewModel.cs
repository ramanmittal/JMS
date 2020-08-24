using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JMS.ViewModels.Review
{
    public class CreateReviewRequestViewModel
    {
        [DisplayName("Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DueDate { get; set; }
        [DisplayName("Review Type")]
        [Required]
        public ReviewType ReviewType { get; set; }
        [DisplayName("Comment For Reviewer")]
        public string EditorComment { get; set; }
        public long SubmissionId { get; set; }
        [Required]
        public long ReviewerId { get; set; }
    }
    public class ReviewerSelectionGridModel
    {
        public int ItemsCount { get; set; }
        public IEnumerable<ReviewerSelectionGridRow> Data { get; set; }
    }
    public class ReviewerSelectionGridRow
    {
        public long ReviewerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public int AssignedRequests { get; set; }
    }
    public class ReviewerSelectionGridSearchModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

        private string srchText;
        public string SrchText
        {
            get { return srchText; }
            set { srchText = value?.Trim(); }
        }
        private string _reviewer;

        public string Reviewer
        {
            get { return _reviewer; }
            set { _reviewer = value?.Trim().ToLower(); }
        }

        public string[] Specialization { get; set; }
        public long submissionID { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
    }
}
