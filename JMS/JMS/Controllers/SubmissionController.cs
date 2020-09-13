using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using ElmahCore;
using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Infra.Sequrity;
using JMS.Models.EmailModels;
using JMS.Models.Submissions;
using JMS.Service.Enums;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using JMS.Setting;
using JMS.ViewModels.Submissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using EditContributerModel = JMS.Models.Submissions.EditContributerModel;

namespace JMS.Controllers
{
    public class SubmissionController : BaseController
    {
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult Edit(long id)
        {
            var submission = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmission(id, ((JMSPrincipal)User).ApplicationUser.Id);
            if (submission.SubmissionStatus != SubmissionStatus.Draft)
            {
                return RedirectToAction("Index",new {id});
            }
            return View(submission.CreateStep);
        }

        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateSubmissionModel createSubmissionModel)
        {
            if (ModelState.IsValid)
            {
                var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
                var submissionId = submissionService.CreateSubmission(createSubmissionModel, ((JMSPrincipal)User).ApplicationUser.Id);
                submissionService.SaveSubmissionHistory(new SubmissionHistory
                {
                    TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                    SubmissionId = submissionId,
                    Action = "Submission Initialization",
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                });
                return RedirectToAction("Edit", new { id = submissionId }); 
            }
            return BadRequest();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.EIC)]
        [NonAction]
        private IActionResult RemoveSubmission(long submissionId)
        {
            var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            submissionService.RemoveSubmission(submissionId, TenantID);
            submissionService.SaveSubmissionHistory(new SubmissionHistory
            {
                TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                SubmissionId = submissionId,
                Action = "Submission has been removed.",
                ActionDate = DateTime.UtcNow,
                ActorEmail = JMSUser.Email,
                ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
            });
            return Ok();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult RemoveSubmissionByAuthor(long submissionId)
        {
            var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            submissionService.RemoveSubmission(submissionId, JMSUser.Id);
            submissionService.SaveSubmissionHistory(new SubmissionHistory
            {
                TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                SubmissionId = submissionId,
                Action = "Submission has been removed.",
                ActionDate = DateTime.UtcNow,
                ActorEmail = JMSUser.Email,
                ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
            });
            return Ok();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult EditMetadata(long id)
        {
            var submission = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmission(id, ((JMSPrincipal)User).ApplicationUser.Id);
            var model = new EditSubmissionMetadataModel()
            {
                Abstract = submission.Abstract,
                Id = submission.Id,
                Keywords = submission.Keywords,
                Prefix = submission.Prefix,
                Subtitle = submission.Subtitle,
                Title = submission.Title
            };
            return PartialView(model);
        }

        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMetadata(EditSubmissionMetadataModel editSubmissionModel)
        {
            if (ModelState.IsValid)
            {
                var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
                submissionService.EditSubmission(editSubmissionModel, ((JMSPrincipal)User).ApplicationUser.Id);
                submissionService.SaveSubmissionHistory(new SubmissionHistory
                {
                    TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                    SubmissionId = editSubmissionModel.Id,
                    Action = "Submission meta data updated",
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                });
                return Ok(); 
            }
            return BadRequest();
        }

        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSubmissionFile(Models.Submissions.AddSubmissionFileModel model)
        {
            if (ModelState.IsValid)
            {
                var mapper = HttpContext.RequestServices.GetService<IMapper>();
                var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
                var listModel = submissionService.AddSubmissionFile(mapper.Map<ViewModels.Submissions.AddSubmissionFileModel>(model));
                submissionService.SaveSubmissionHistory(new SubmissionHistory
                {
                    TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                    SubmissionId = model.SubmissionId,
                    Action = $"File {model.File.FileName} has been added.",
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                });
                return Ok(listModel);
            }
            return BadRequest();

        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult AddSubmissionFile(long id)
        {
            return PartialView(new AddSubmissionFileViewModel
            {
                ArticleComponenets = HttpContext.RequestServices.GetService<ISubmissionService>().GetArticleComponent(TenantID).Select(x => new SelectListItem(x.Value, x.Key.ToString())),
                SubmissionId = id
            });
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult EditSubmissionFile(long id)
        {
            var submissionFile = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmissionFile(id, ((JMSPrincipal)User).ApplicationUser.Id);
            return PartialView(new EditSubmissionFileModel
            {
                ArticleComponentId = submissionFile.ArticleComponentId,
                SubmissionFileId = submissionFile.Id,
                Description = submissionFile.Description,
                Creator = submissionFile.Creator,
                Subject = submissionFile.Subject
            });
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSubmissionFile(EditSubmissionFileModel model)
        {
            if (ModelState.IsValid)
            {
                var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
                submissionService.SaveSubmissionFile(model, ((JMSPrincipal)User).ApplicationUser.Id);                
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSubmissionFile(long Id)
        {
            var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            var file = submissionService.GetSubmissionFile(Id, JMSUser.Id);
            submissionService.RemoveFile(file, ((JMSPrincipal)User).ApplicationUser.Id);
            submissionService.SaveSubmissionHistory(new SubmissionHistory
            {
                TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                SubmissionId = file.SubmissionId,
                Action = $"File {file.FileName} has been removed.",
                ActionDate = DateTime.UtcNow,
                ActorEmail = JMSUser.Email,
                ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
            });
            return Ok();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult Submissionfiles(long id)
        {
            return PartialView();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult GetSubmissionfiles(long id)
        {
            var files = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmissionFiles(id, ((JMSPrincipal)User).ApplicationUser.Id);
            return Ok(files);
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult DownloadSubmissionFile(long id)
        {
            var submissionFile = HttpContext.RequestServices.GetService<ISubmissionService>().
            GetSubmissionFile(id, ((JMSPrincipal)User).ApplicationUser.Id);
            var bytes = HttpContext.RequestServices.GetService<IFileService>().GetFileBytes(submissionFile.FileId);

            return File(bytes, "application/pdf", submissionFile.FileName);
        }
        [Authorize(Roles =RoleName.EditorRoles)]
        [HttpGet]
        public IActionResult DownloadSubmittedFile(long id)
        {
            var submissionFile = HttpContext.RequestServices.GetService<ISubmissionService>().
            GetSubmissionFile(id, TenantID);
            var bytes = HttpContext.RequestServices.GetService<IFileService>().GetFileBytes(submissionFile.FileId);

            return File(bytes, "application/pdf", submissionFile.FileName);
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult MovetoContributer(long id)
        {
            HttpContext.RequestServices.GetService<ISubmissionService>().MovetoContributer(id, ((JMSPrincipal)User).ApplicationUser.Id);
            return Ok();
        }

        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult AddContributer(long id)
        {
            HttpContext.RequestServices.GetService<ISubmissionService>().MovetoContributer(id, ((JMSPrincipal)User).ApplicationUser.Id);
            var model = new AddContributerModel();
            model.SubmissionId = id;
            return PartialView(model);
        }

        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult ValidateContributerEmail(string email, long submissionId, long? contributerId)
        {
            return Json(data: HttpContext.RequestServices.GetService<ISubmissionService>().ValidateContributerEmail(email, submissionId, ((JMSPrincipal)User).ApplicationUser, contributerId));
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostAddContirbuter(AddContributerModel model) {
            if (ModelState.IsValid)
            {
                var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
                var contibuterViewModel = HttpContext.RequestServices.GetService<IMapper>().Map<AddContributerViewModel>(model);
                submissionService.AddContributer(contibuterViewModel);
                submissionService.SaveSubmissionHistory(new SubmissionHistory
                {
                    TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                    SubmissionId = model.SubmissionId,
                    Action = $"Contributer {model.FirstName} {model.LastName} has been added.",
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                });
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult ContributersView(long id)
        {
            return PartialView(id);
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult ContributersJson(long submissionId)
        {
            return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().Contributers(submissionId, ((JMSPrincipal)User).ApplicationUser.Id)
            );
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        
        public IActionResult EditContributer(long id)
        {
            var contributer = HttpContext.RequestServices.GetService<ISubmissionService>().GetContributor(id, ((JMSPrincipal)User).ApplicationUser.Id);
            return PartialView(new Models.Submissions.EditContributerModel
            {
                AffiliationNo=contributer.AffiliationNo,
                ContributerId=contributer.Id,
                SubmissionId=contributer.SubmissionId,
                ContributerRole=contributer.ContributerRole,
                Country=contributer.Country,
                Email=contributer.Email,
                FirstName=contributer.FirstName,
                LastName=contributer.LastName,
                ORCIDiD=contributer.ORCIDiD
            });
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditContributer(EditContributerModel model)
        {
            if (ModelState.IsValid)
            {
                var contibuterViewModel = HttpContext.RequestServices.GetService<IMapper>().Map<JMS.ViewModels.Submissions.EditContributerModel>(model);
                var submissionService=HttpContext.RequestServices.GetService<ISubmissionService>();
                submissionService.EditContributer(contibuterViewModel, ((JMSPrincipal)User).ApplicationUser.Id);
                submissionService.SaveSubmissionHistory(new SubmissionHistory
                {
                    TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                    SubmissionId = model.SubmissionId,
                    Action = $"Contributer {model.FirstName} {model.LastName} has been Updated.",
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                });
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveContributor(long contributerId)
        {
            var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            var contributer = submissionService.GetContributor(contributerId, JMSUser.Id);
            submissionService.DeleteContributor(contributer);
            submissionService.SaveSubmissionHistory(new SubmissionHistory
            {
                TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                SubmissionId = contributer.SubmissionId,
                Action = $"Contributer {contributer.FirstName} {contributer.LastName} has been removed.",
                ActionDate = DateTime.UtcNow,
                ActorEmail = JMSUser.Email,
                ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
            });
            return Ok();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MovetoFinish(long id)
        {
            HttpContext.RequestServices.GetService<ISubmissionService>().MovetoFinish(id, ((JMSPrincipal)User).ApplicationUser.Id);
            return Ok();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpGet]
        public IActionResult EditorComment(long id)
        {
            var submission = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmission(id, ((JMSPrincipal)User).ApplicationUser.Id);
            return PartialView(new EditorCommentModel {Id=id,Comment=submission.EditorComment });
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditorComment(EditorCommentModel model)
        {
            if (ModelState.IsValid)
            {
                var sumissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
                var submission = sumissionService.GetSubmission(model.Id, ((JMSPrincipal)User).ApplicationUser.Id);
                if (submission.SubmissionStatus==SubmissionStatus.Draft)
                {
                    sumissionService.EditorComment(submission, model, ((JMSPrincipal)User).ApplicationUser.Id);
                    sumissionService.SaveSubmissionHistory(new SubmissionHistory
                    {
                        TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                        SubmissionId = model.Id,
                        Action = $"Editor comment has been changed.",
                        ActionDate = DateTime.UtcNow,
                        ActorEmail = JMSUser.Email,
                        ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                    });
                    if (model.IsFinished.HasValue && model.IsFinished.Value)
                    {
                        try
                        {
                           await SubmissionConfirmaitionEmail(model.Id, GetService<IRazorViewToStringRenderer>(), GetService<IEmailSender>(), GetService<IFileService>());
                            AddSuccessMessage("Submission has been submitted. A confirmation email has been send to you.");
                            sumissionService.SaveSubmissionHistory(new SubmissionHistory
                            {
                                TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                                SubmissionId = model.Id,
                                Action = $"Submission has been Completed and Submitted",
                                ActionDate = DateTime.UtcNow,
                                ActorEmail = JMSUser.Email,
                                ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                            });
                        }
                        catch (Exception ex)
                        {
                            HttpContext.RiseError(ex);
                            AddFailMessage("Submission has been submitted  but failed to send confirmation email.");
                        }
                       
                        try
                        {
                            var eics = GetService<IUserService>().GetTenantUserByRole(JMSUser.TenantId.Value, RoleName.EIC);
                            if (eics.Any())
                            {
                                var emailBody = await GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/EmailTemplates/AddSubmissionNotificationEmail.cshtml", new AddSubmisssionNotificationEmailModel());
                                var mailMessage = new MailMessage(_configuration[JMSSetting.SenderEmail], eics.First().Email, _configuration[JMSSetting.NewSubmissionEmailSubject], emailBody) { IsBodyHtml = true };
                                foreach (var eic in eics.Skip(1))
                                {
                                    mailMessage.To.Add(eic.Email);
                                }
                                GetService<IEmailSender>().SendEmail(mailMessage);
                            }                            
                        }
                        catch (Exception ex)
                        {
                            HttpContext.RiseError(ex);
                        }
                    }
                }
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult GetSubmission(SubmissionGridSearchModel model)
        {
            return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmissions(((JMSPrincipal)User).ApplicationUser.Id, model));
        }
        [Authorize(Roles = RoleName.EIC)]
        [HttpGet]
        public IActionResult ActiveSubmission()
        {
            var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            var model = submissionService.SubmissionCount(TenantID);
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            model.Editors = userService.GetJounalEditors(TenantID);

            return View(model);
        }
        [Authorize(Roles = RoleName.EditorRoles)]
        [HttpGet]
        public IActionResult MyAssigned()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult GetActiveSubmission(EditorSubmissionGridSearchModel model)
        {
            if (User.IsInRole(RoleName.SectionEditor))
            {
                return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().JournalSubmission(TenantID, model, ((JMSPrincipal)User).ApplicationUser.Id));
            }
            return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().JournalSubmission(TenantID, model));
        }

        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        [ActionName("View")]
        public IActionResult SubmissionView(long id)
        {
            var status = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmission(id, journalPath: TenantID).SubmissionStatus;
            return View((int)status);
        }

        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult SubmissionDetails(long id)
        {
            var model = HttpContext.RequestServices.GetService<ISubmissionService>().GetEditorSubmissionViewModel(id, TenantID);
            var userService = HttpContext.RequestServices.GetService<IUserService>();
            model.Editors = userService.GetJounalEditors(TenantID);
            return PartialView(model);
        }

        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult FileDetails(long id)
        {
            var submissionFile = HttpContext.RequestServices.GetService<ISubmissionService>().SubmisssionFileDetails(id, TenantID);            
            return PartialView(submissionFile);
        }
        [HttpPost]
        [Authorize(Roles = RoleName.EIC)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEditor(long submissionId,long? editorId)
        {
            var sumissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            sumissionService.AssignEditor(submissionId, editorId, TenantID);
            ApplicationUser editor = null;
            if (editorId.HasValue)
            {
                editor = HttpContext.RequestServices.GetService<IUserService>().GetUser(editorId.Value);
            }
            if (editorId != JMSUser.Id)
            {
                var notificationModel = new EditorAssignmentNotificationEmailModel
                {
                    FirstName = editor.FirstName,
                    LastName = editor.LastName
                };
                try
                {
                    var emailBody = await GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/EmailTemplates/EditorAssignmentNotificationEmail.cshtml", notificationModel);
                    var mailMessage = new MailMessage(_configuration[JMSSetting.SenderEmail], editor.Email, _configuration[JMSSetting.AssignEditorEmailSubject], emailBody) { IsBodyHtml = true };
                    GetService<IEmailSender>().SendEmail(mailMessage);
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(ex);
                }
            }
            sumissionService.SaveSubmissionHistory(new SubmissionHistory
            {
                TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                SubmissionId = submissionId,
                Action = editorId.HasValue ? $"Editor {editor?.FirstName} {editor?.LastName} has been assigned." : $"Assigned editor has been removed.",
                ActionDate = DateTime.UtcNow,
                ActorEmail = JMSUser.Email,
                ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
            });
            return Ok();
        }
        [HttpPost]
        [Authorize(Roles = RoleName.EditorRoles)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToReview(long id)
        {
            var submissionService = HttpContext.RequestServices.GetService<ISubmissionService>();
            submissionService.MoveToReview(id, TenantID);
            try
            {
                submissionService.SaveSubmissionHistory(new SubmissionHistory
                {
                    TenanatID = JMSUser.TenantId.GetValueOrDefault(),
                    SubmissionId = id,
                    Action = "Submission has been put in review.",
                    ActionDate = DateTime.UtcNow,
                    ActorEmail = JMSUser.Email,
                    ActorName = $"{JMSUser.FirstName} {JMSUser.LastName}"
                });
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(ex);
            }
            try
            {
                await SubmissionReviewNotificationEmail(id);
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(ex);
                return Ok(new { EmailError = true });
            }
            return Ok();
        }

        public async Task<IActionResult> SubmissionReviewNotificationEmail(long submissionId)
        {
            var submission = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmission(submissionId, journalPath: TenantID);
            var userID = submission.UserID;
            var user = HttpContext.RequestServices.GetService<IUserService>().GetUser(userID);
            var emailBody = await HttpContext.RequestServices.GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/Submission/SubmissionReviewNotificationEmail.cshtml", new SubmissionReviewEmailModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            });
            var mailMessage = new MailMessage(_configuration[JMSSetting.SenderEmail], user.Email, _configuration[JMSSetting.SubmissionReviewSubject], emailBody) { IsBodyHtml = true };

            HttpContext.RequestServices.GetService<IEmailSender>().SendEmail(mailMessage);
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = RoleName.EditorRoles)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectSubmission(RejectSubmission rejectSubmission)
        {
            HttpContext.RequestServices.GetService<ISubmissionService>().RejectSubmission(rejectSubmission, TenantID);
            try
            {
                await SubmissionRejectionEmail(rejectSubmission.Id);
                AddSuccessMessage("Submission has been rejected.");
                return Ok();
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(ex);
                AddFailMessage("Submission has been rejected but failed to send confirmation email to author.");
                return Ok();
            }

        }
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult GetRejectedSubmission(RejectedSubmissionGridSearchModel model)
        {
            if (User.IsInRole(RoleName.SectionEditor))
            {
                return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().GetRejectedSubmissions(TenantID, model, ((JMSPrincipal)User).ApplicationUser.Id));
            }
            return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().GetRejectedSubmissions(TenantID, model));
        }
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult RejectedSubmission()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        public IActionResult Index(int id)
        {
            var model = HttpContext.RequestServices.GetService<ISubmissionService>().GetAuthorSubmissionViewModel(id, (((JMSPrincipal)User).ApplicationUser.Id), TenantID);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> SubmissionConfirmaitionEmail(long id, [FromServices] IRazorViewToStringRenderer _razorViewToStringRenderer, [FromServices] IEmailSender _emailSender, [FromServices]IFileService fileService)
        {
            var model = HttpContext.RequestServices.GetService<ISubmissionService>().GetAuthorSubmissionViewModel(id, (((JMSPrincipal)User).ApplicationUser.Id), TenantID);
            var emailBody = await _razorViewToStringRenderer.RenderViewToStringAsync(@"/Views/Submission/SubmissionConfirmaitionEmail.cshtml", model);
            var mailMessage = new MailMessage(_configuration[JMSSetting.SenderEmail], ((JMSPrincipal)User).ApplicationUser.Email, _configuration[JMSSetting.SubmissionConfirmationSubject], emailBody) { IsBodyHtml = true };
            foreach (var file in model.Files)
            {
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(fileService.GetFileBytes(file.FileId)), file.FileName));
            }
            model.Contributers.ForEach(x =>
            {
                mailMessage.CC.Add(x.Email);
            });
            _emailSender.SendEmail(mailMessage);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> SubmissionRejectionEmail(long submissionid)
        {
            var submission = HttpContext.RequestServices.GetService<ISubmissionService>().GetSubmission(submissionid, journalPath: TenantID);
            var user = HttpContext.RequestServices.GetService<IUserService>().GetUser(submission.UserID);
            var emailBody = await HttpContext.RequestServices.GetService<IRazorViewToStringRenderer>().RenderViewToStringAsync(@"/Views/Submission/SubmissionRejectionEmail.cshtml", new SubmissionRejectionEmailModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                RejectionMessage = submission.RejectComment,
            });
            var mailMessage = new MailMessage(_configuration[JMSSetting.SenderEmail], user.Email, _configuration[JMSSetting.SubmissionRejectSubject], emailBody) { IsBodyHtml = true };

            HttpContext.RequestServices.GetService<IEmailSender>().SendEmail(mailMessage);
            return Ok();
        }
        [HttpGet]
        [Authorize(Roles = RoleName.EditorRoles)]
        public IActionResult GetActivityLogs(long submissionId)
        {
            return PartialView(HttpContext.RequestServices.GetService<ISubmissionService>().GetActivityLogs(submissionId, JMSUser.TenantId.Value));
        }

    }
}