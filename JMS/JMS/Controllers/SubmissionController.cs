using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using JMS.Infra.Sequrity;
using JMS.Models.Submissions;
using JMS.Service.Enums;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using JMS.ViewModels.Submissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return View(submission.CreateStep);
        }

        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateSubmissionModel createSubmissionModel)
        {
            if (ModelState.IsValid)
            {
                var submissionId = HttpContext.RequestServices.GetService<ISubmissionService>().CreateSubmission(createSubmissionModel, ((JMSPrincipal)User).ApplicationUser.Id);
                return RedirectToAction("Edit", new { id = submissionId }); 
            }
            return BadRequest();
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
                HttpContext.RequestServices.GetService<ISubmissionService>().EditSubmission(editSubmissionModel, ((JMSPrincipal)User).ApplicationUser.Id);
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
                var listModel = HttpContext.RequestServices.GetService<ISubmissionService>().AddSubmissionFile(mapper.Map<ViewModels.Submissions.AddSubmissionFileModel>(model));
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
                HttpContext.RequestServices.GetService<ISubmissionService>().SaveSubmissionFile(model, ((JMSPrincipal)User).ApplicationUser.Id);
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSubmissionFile(long Id)
        {
            HttpContext.RequestServices.GetService<ISubmissionService>().RemoveFile(Id, ((JMSPrincipal)User).ApplicationUser.Id);
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
                var contibuterViewModel = HttpContext.RequestServices.GetService<IMapper>().Map<AddContributerViewModel>(model);
                HttpContext.RequestServices.GetService<ISubmissionService>().AddContributer(contibuterViewModel);
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
                HttpContext.RequestServices.GetService<ISubmissionService>().EditContributer(contibuterViewModel, ((JMSPrincipal)User).ApplicationUser.Id);
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Policy = AuthorPolicyRequirementHandler.AuthorPolicy)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveContributor(long contributerId)
        {
            HttpContext.RequestServices.GetService<ISubmissionService>().DeleteContributor(contributerId, ((JMSPrincipal)User).ApplicationUser.Id);
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
        public IActionResult EditorComment(EditorCommentModel model)
        {
            if (ModelState.IsValid)
            {
                HttpContext.RequestServices.GetService<ISubmissionService>().EditorComment(model, ((JMSPrincipal)User).ApplicationUser.Id);
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
        [HttpGet]
        public IActionResult GetActiveSubmission(EditorSubmissionGridSearchModel model)
        {
            return Ok(HttpContext.RequestServices.GetService<ISubmissionService>().JournalSubmission(TenantID, model));
        }

    }
}