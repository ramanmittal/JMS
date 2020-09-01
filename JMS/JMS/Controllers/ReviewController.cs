using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using JMS.Helpers;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using JMS.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace JMS.Controllers
{
    public class ReviewController : BaseController
    {
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult ReviewRequests(long submissionId)
        {
            return View(submissionId);
        }
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult ReviewRequestGridData(long submissionId)
        {
            var data = HttpContext.RequestServices.GetService<IReviewService>().GetReviewRequestGridData(submissionId, TenantID);
            return Ok(data);
        }
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult CreateReviewer(long submissionId)
        {
            var model = new CreateReviewRequestViewModel {SubmissionId=submissionId };
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult SelectReviewGridData()
        {
            string json = new StreamReader(Request.Body).ReadToEnd();
            var model = JsonConvert.DeserializeObject<ReviewerSelectionGridSearchModel>(json);
            var data = HttpContext.RequestServices.GetService<IReviewService>().ReviewerGridSubmission(model, TenantID);
            return Ok(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult AssignReviewer(CreateReviewRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                HttpContext.RequestServices.GetService<IReviewService>().AssignReviewer(model, TenantID);
                var reviewer = HttpContext.RequestServices.GetService<IUserService>().GetUser(model.ReviewerId);
                HttpContext.RequestServices.GetService<ISubmissionService>().SaveSubmissionHistory(new SubmissionHistory
                {
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.FirstName}",
                    SubmissionId = model.SubmissionId,
                    TenanatID = JMSUser.TenantId.Value,
                    Action = $"{reviewer.FirstName} {reviewer.LastName} has been assigned to the Submission Review Type: {model.ReviewType.ToString()} {(model.DueDate.HasValue ? $"with due date {model.DueDate.Value.ToString("dd MMM yyyy")}." : ".")}"
                });
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult RemoveReviewRequest(long requestID)
        {
            var reviewService = HttpContext.RequestServices.GetService<IReviewService>();
            var requestReview = reviewService.GetReviewRequest(requestID, TenantID);
            reviewService.RemoveReviewRequest(requestID, TenantID);
            var reviewer = HttpContext.RequestServices.GetService<IUserService>().GetUser(requestReview.ReviewerID);
            HttpContext.RequestServices.GetService<ISubmissionService>().SaveSubmissionHistory(new SubmissionHistory
            {
                ActionDate = DateTime.UtcNow,
                ActorEmail = JMSUser.Email,
                ActorName = $"{JMSUser.FirstName} {JMSUser.FirstName}",
                SubmissionId = requestReview.SubmissionId,
                TenanatID = JMSUser.TenantId.Value,
                Action = $"Review request with reviewer {reviewer.FirstName} {reviewer.LastName} has been deleted."
            });
            return Ok();
        }
    }
}