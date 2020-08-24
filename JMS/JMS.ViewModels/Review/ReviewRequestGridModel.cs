using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Review
{
    public class ReviewRequestGridModel
    {
        public int ItemsCount { get; set; }
        public IEnumerable<ReviewRequestGridModelItem> Data { get; set; }
    }
    public class ReviewRequestGridModelItem
    {
        public long Id { get; set; }
        public string ReviewerName { get; set; }
        public string CreatedDate { get; set; }
        public string DueDate { get; set; }
        public string ReviewType { get; set; }
    }
}
