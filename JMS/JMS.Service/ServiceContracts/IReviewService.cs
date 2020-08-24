using JMS.ViewModels.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface IReviewService
    {
        ReviewerSelectionGridModel ReviewerGridSubmission(ReviewerSelectionGridSearchModel searchModel, string journalpath);
        IEnumerable<ReviewRequestGridModelItem> GetReviewRequestGridData(long submissionId, string journalPath);
        void AssignReviewer(CreateReviewRequestViewModel model, string journalpath);
        void RemoveReviewRequest(long requestId, string journalpath);
    }
}
