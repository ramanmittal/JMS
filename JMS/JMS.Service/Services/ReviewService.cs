using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Service.Enums;
using JMS.Service.ServiceContracts;
using JMS.ViewModels.Review;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Z.EntityFramework.Plus;

namespace JMS.Service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IServiceProvider _serviceProvider;
        public ReviewService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IEnumerable<ReviewRequestGridModelItem> GetReviewRequestGridData(long submissionId, string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            return context.ReviewRequest.Where(x => x.SubmissionId == submissionId && x.Submission.User.Tenant.JournalPath == journalPath).Select(x => new
            {
                x.ID,
                x.Reviewer.FirstName,
                x.Reviewer.LastName,
                x.CreatedDate,
                x.DueDate,
                x.ReviewType
            }).ToList().Select(x => new ReviewRequestGridModelItem
            {
                ReviewerName = $"{x.FirstName} {x.LastName}",
                CreatedDate = x.CreatedDate.ToString("dd MMM yyyy"),
                DueDate = x.DueDate?.ToString("dd MMM yyyy"),
                Id = x.ID,
                ReviewType = x.ReviewType.ToString()
            });
        }
        public ReviewerSelectionGridModel ReviewerGridSubmission(ReviewerSelectionGridSearchModel searchModel, string journalpath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var reviewerRoleId = context.Roles.Where(x => x.Name == Role.Reviewer.ToString()).Select(x => x.Id).First();
            var existingrevierIds = context.ReviewRequest.Where(x => x.SubmissionId == searchModel.submissionID).Select(x => x.ReviewerID);

            var reviewers = (from user in context.Users.Where(x => !existingrevierIds.Contains(x.Id) && x.IsDisabled != true && x.Tenant.JournalPath.ToLower() == journalpath.ToLower())
                             join
                             userrole in context.UserRoles.Where(x => x.RoleId == reviewerRoleId) on user.Id equals userrole.UserId
                             select user);
            if (!string.IsNullOrEmpty(searchModel.Reviewer))
            {
                reviewers = reviewers.Where(x => (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(searchModel.Reviewer));
            }
            var skills = searchModel.Specialization?.Where(x => !string.IsNullOrEmpty(x));
            if (skills != null && skills.Any())
            {

                Expression<Func<ApplicationUser, bool>> predicate = f => false;
                foreach (var item in skills)
                {
                    Expression<Func<ApplicationUser, bool>> predicate1 = x => x.Specialization.ToLower().Contains(item.ToLower());
                    var invokedExpr = Expression.Invoke(predicate1, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<ApplicationUser, bool>>(Expression.OrElse(predicate.Body, invokedExpr), predicate.Parameters);
                }
                reviewers = reviewers.Where(predicate);
            }
            var submissions = context.ReviewRequest.Where(x => x.SubmissionId != searchModel.submissionID && x.Submission.User.Tenant.JournalPath == journalpath).GroupBy(x => x.ReviewerID).Select(x => new { ReviewerID = x.Key, Assigned = x.Count() });
            var query = from reviewer in reviewers
                        join pet in submissions on reviewer.Id equals pet.ReviewerID into gj
                        from subpet in gj.DefaultIfEmpty()
                        select new ReviewerSelectionGridRow { FirstName = reviewer.FirstName, LastName = reviewer.LastName, Email = reviewer.Email, ReviewerId = reviewer.Id, AssignedRequests = subpet.Assigned, Specialization = reviewer.Specialization };
            if (searchModel.sortField == "AssignedRequest")
            {
                if (searchModel.sortOrder == "asc")
                {
                    query = query.OrderBy(x => x.AssignedRequests);
                }
                else
                {
                    query = query.OrderByDescending(x => x.AssignedRequests);
                }
            }
            return new ReviewerSelectionGridModel
            {
                ItemsCount = query.Count(),
                Data = query.Skip((searchModel.pageIndex - 1) * searchModel.pageSize).Take(searchModel.pageSize).ToList()
            };

        }

        public void AssignReviewer(CreateReviewRequestViewModel model, string journalpath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            if (!context.ReviewRequest.Any(x => x.SubmissionId == model.SubmissionId && x.ReviewerID == model.ReviewerId))
            {
                context.ReviewRequest.Add(new ReviewRequest
                {
                    CreatedDate = DateTime.UtcNow,
                    DueDate= model.DueDate,
                    EditorComment=model.EditorComment,
                    ReviewerID=model.ReviewerId,
                    ReviewType=model.ReviewType,
                    SubmissionId= model.SubmissionId
                });
                _=context.SaveChanges();
            }

        }

        public void RemoveReviewRequest(long requestId,string journalpath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            context.ReviewRequest.Where(x => x.ID == requestId && x.Submission.User.Tenant.JournalPath == journalpath).Delete();
        }
    }
}
