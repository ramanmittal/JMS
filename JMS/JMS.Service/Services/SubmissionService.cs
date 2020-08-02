using JMS.Entity.Data;
using JMS.Service.ServiceContracts;
using JMS.ViewModels.Submissions;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using JMS.Entity.Entities;
using System.Linq;
using JMS.Entity.Migrations;
using SubmisssionFile = JMS.Entity.Entities.SubmisssionFile;
using Contributor = JMS.Entity.Entities.Contributor;

namespace JMS.Service.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IServiceProvider _serviceProvider;
        public SubmissionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public long CreateSubmission(CreateSubmissionModel createSubmissionModel, long userId)
        {
            var submission = new Submission();
            submission.Prefix = createSubmissionModel.Prefix;
            submission.Title = createSubmissionModel.Title;
            submission.Subtitle = createSubmissionModel.Subtitle;
            submission.SubmissionStatus = SubmissionStatus.Draft;
            submission.Abstract = createSubmissionModel.Abstract;
            submission.Keywords = createSubmissionModel.Keywords;
            submission.UserID = userId;
            submission.UpdatedDate = DateTime.UtcNow;
            submission.CreateStep = 1;
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            context.Submission.Add(submission);
            context.SaveChanges();
            return submission.Id;
        }
        public SubmissionFileListModel AddSubmissionFile(AddSubmissionFileModel model)
        {
            var fileName = _serviceProvider.GetService<IFileService>().SaveFile(model.FileStream, model.FileName);
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission = context.Submission.First(x => x.Id == model.SubmissionId);
            submission.UpdatedDate = DateTime.UtcNow;
            var file = new SubmisssionFile
            {
                ArticleComponentId = model.ArticleComponentId,
                FileId = fileName,
                FileName = model.FileName,
                UploadedOn = DateTime.UtcNow,
                Description = model.Description,
                Subject = model.Subject,
                SubmissionId = model.SubmissionId,
            };
            context.SubmisssionFile.Add(file);
            context.SaveChanges();
            var component = context.TenantArticleComponent.First(x => x.Id == model.ArticleComponentId).Text;
            return new SubmissionFileListModel { FileName = model.FileName, SubmissionFileID = file.Id, UploadDate = file.UploadedOn.ToString("dd MMM yyyy"), ArticalComponent = component };
        }

        public Dictionary<long, string> GetArticleComponent(string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            return context.TenantArticleComponent.Where(x => x.Tenant.JournalPath == journalPath).OrderBy(x => x.Order).ToDictionary(x => x.Id, x => x.Text);
        }

        public Submission GetSubmission(long SubmissionId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission = context.Submission.First(x => x.Id == SubmissionId && x.UserID == userId);
            return submission;
        }

        public IEnumerable<SubmissionFileListModel> GetSubmissionFiles(long submissionId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissionFiles = context.SubmisssionFile.Where(x => x.SubmissionId == submissionId && x.Submission.UserID == userId).Select(x => new
            {
                ArticalComponent = x.TenantArticleComponent.Text,
                FileName = x.FileName,
                UploadDate = x.UploadedOn,
                SubmissionFileID = x.Id
            }).OrderByDescending(x => x.UploadDate).ToList().Select(x => new SubmissionFileListModel
            {
                ArticalComponent = x.ArticalComponent,
                FileName = x.FileName,
                UploadDate = x.UploadDate.ToString("dd MMM yyyy"),
                SubmissionFileID = x.SubmissionFileID
            });
            return submissionFiles;
        }

        public SubmisssionFile GetSubmissionFile(long submissionFileId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissionFile = context.SubmisssionFile.Where(x => x.Id == submissionFileId && x.Submission.UserID == userId).First();
            return submissionFile;
        }
        public void SaveSubmissionFile(EditSubmissionFileModel model, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var file = context.SubmisssionFile.First(x => x.Submission.UserID == userId && x.Id == model.SubmissionFileId);
            file.Creator = model.Creator;
            file.ArticleComponentId = model.ArticleComponentId;
            file.Description = model.Description;
            file.Subject = model.Subject;
            context.SaveChanges();
        }

        public void RemoveFile(long submissionFileId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var file = context.SubmisssionFile.First(x => x.Submission.UserID == userId && x.Id == submissionFileId);
            var submission = context.Submission.First(x => x.Id == file.SubmissionId);
            submission.UpdatedDate = DateTime.UtcNow;
            context.SubmisssionFile.Remove(file);
            context.SaveChanges();
        }
        public void EditSubmission(EditSubmissionMetadataModel model, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission = context.Submission.First(x => x.UserID == userId && x.Id == model.Id);
            submission.Prefix = model.Prefix;
            submission.Title = model.Title;
            submission.Subtitle = model.Subtitle;
            submission.Abstract = model.Abstract;
            submission.Keywords = model.Keywords;
            submission.UpdatedDate = DateTime.UtcNow;
            context.SaveChanges();
        }

        public void MovetoContributer(long submissionId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission = context.Submission.First(x => x.UserID == userId && x.Id == submissionId);
            submission.CreateStep = Math.Max(2, submission.CreateStep);
            context.SaveChanges();
        }
        public void MovetoFinish(long submissionId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission = context.Submission.First(x => x.UserID == userId && x.Id == submissionId);
            submission.SubmissionStatus = SubmissionStatus.Submission;            
            submission.CreateStep = Math.Max(3, submission.CreateStep);
            context.SaveChanges();
        }
        public bool ValidateContributerEmail(string email, long submissionID, ApplicationUser user, long? contributerId)
        {
            if (email == user.Email)
            {
                return false;
            }
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var contributers = context.Contributors.Where(x => x.SubmissionId == submissionID && x.Submission.UserID == user.Id);
            if (contributerId.HasValue)
            {
                contributers = contributers.Where(x => x.Id != contributerId.Value);
            }
            var contributerEmails = contributers.Select(x => x.Email).ToArray();
            return !contributerEmails.Contains(email);
        }
        public void AddContributer(AddContributerViewModel model)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            context.Contributors.Add(new Contributor
            {
                AddedOn = DateTime.UtcNow,
                AffiliationNo = model.AffiliationNo,
                ContributerRole = model.ContributerRole,
                Country = model.Country,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ORCIDiD = model.ORCIDiD,
                SubmissionId = model.SubmissionId
            });
            context.Submission.First(x => x.Id == model.SubmissionId).UpdatedDate = DateTime.UtcNow;
            context.SaveChanges();
        }
        public void EditContributer(EditContributerModel model, long? userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var contributers = context.Contributors.Where(x => x.Id == model.ContributerId);
            if (userId.HasValue)
            {
                contributers = contributers.Where(x => x.Submission.UserID == userId);
            }
            var contributer = contributers.First();
            contributer.FirstName = model.FirstName;
            contributer.LastName = model.LastName;
            contributer.Email = model.Email;
            contributer.Country = model.Country;
            contributer.ORCIDiD = model.ORCIDiD;
            contributer.AffiliationNo = model.AffiliationNo;
            contributer.ContributerRole = model.ContributerRole;
            context.SaveChanges();
        }
        public IEnumerable<ContributerListModel> Contributers(long submissionId, long? userID)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var contributers = context.Contributors.Where(x => x.SubmissionId == submissionId);
            if (userID.HasValue)
            {
                contributers = contributers.Where(x => x.Submission.UserID == userID.Value);
            }
            return contributers.Select(x => new ContributerListModel { ContributerId = x.Id, Email = x.Email, FirstName = x.FirstName, LastName = x.LastName, Role = x.ContributerRole.ToString() }).ToList();
        }
        public Contributor GetContributor(long contributerId, long? userID)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var contributers = context.Contributors.Where(x => x.Id == contributerId);
            if (userID.HasValue)
            {
                contributers = contributers.Where(x => x.Submission.UserID == userID.Value);
            }
            return contributers.FirstOrDefault();
        }
        public void DeleteContributor(long contributerId, long? userID)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var contributers = context.Contributors.Where(x => x.Id == contributerId);
            if (userID.HasValue)
            {
                contributers = contributers.Where(x => x.Submission.UserID == userID.Value);
            }
            var contributer = contributers.FirstOrDefault();
            context.Submission.First(x => x.Id == contributer.SubmissionId).UpdatedDate = DateTime.UtcNow;
            context.Contributors.Remove(contributer);
            context.SaveChanges();
        }

        public void EditorComment(EditorCommentModel model, long? userID)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == model.Id);
            if (userID.HasValue)
            {
                submissions = submissions.Where(x => x.UserID == userID.Value);
            }
            var submission = submissions.FirstOrDefault();
            submission.EditorComment = model.Comment;
            context.SaveChanges();
        }
    }
}
