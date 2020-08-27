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
using JMS.ViewModels.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using JMS.ViewModels.Enums;
using Z.EntityFramework.Plus;

namespace JMS.Service.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IServiceProvider _serviceProvider;
        public SubmissionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void RemoveSubmission(long submissionId, long userId)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission = context.Submission.First(x => x.Id == submissionId && x.UserID == userId);
            RemoveSubmission(submission);
        }
        public void RemoveSubmission(long submissionId, string path) {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submission=context.Submission.First(x => x.Id == submissionId && x.User.Tenant.JournalPath == path);
            RemoveSubmission(submission);
        }
        private void RemoveSubmission(Submission submission)
        {
            long submissionID = submission.Id;
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var files = context.SubmisssionFile.Where(x => x.SubmissionId == submissionID);
            var contributers = context.Contributors.Where(x => x.SubmissionId == submissionID);            
            using (var tr = context.Database.BeginTransaction())
            {
                try
                {
                    files.Delete();
                    contributers.Delete();
                    context.Submission.Where(x => x.Id == submissionID).Delete();
                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
            }
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
            submission.CreatedDate = DateTime.UtcNow;
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

        public Submission GetSubmission(long SubmissionId, long? userId = null, string journalPath = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == SubmissionId);
            if (userId.HasValue)
            {
                submissions = submissions.Where(x => x.UserID == userId);
            }
            if (!string.IsNullOrEmpty(journalPath))
            {
                submissions = submissions.Where(x => x.User.Tenant.JournalPath == journalPath);
            }
            var submission = submissions.First();
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
            submission.CreateStep = Math.Max(1, submission.CreateStep);
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
            var submissions = context.Submission.Where(x => x.Id == contributer.SubmissionId);
            if (userId.HasValue)
            {
                submissions = submissions.Where(x => x.UserID == userId);
            }
            submissions.First().UpdatedDate = DateTime.UtcNow;
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
            var previoursStatus = submission.SubmissionStatus;
            if (model.IsFinished == true)
            {
                submission.SubmissionStatus = SubmissionStatus.Submission;
            }            
            context.SaveChanges();
            
        }

        public SubmissionGridModel GetSubmissions(long userID, SubmissionGridSearchModel model)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var allSubmission = context.Submission.Where(x => x.UserID == userID);
            
            if (!string.IsNullOrEmpty(model.Title))
            {
                allSubmission = allSubmission.Where(x => EF.Functions.ILike(x.Title, model.Title));
            }
            if (!string.IsNullOrEmpty(model.Keywords))
            {
                allSubmission = allSubmission.Where(x => EF.Functions.ILike(x.Keywords, $"%{model.Keywords}%"));
            }
            if (model.Status.HasValue)
            {
                allSubmission = allSubmission.Where(x => x.SubmissionStatus == model.Status.Value);
            }
            var sortField = "UpdatedDate";
            var sortOrder = "desc";
            if (!string.IsNullOrEmpty(model.sortField) && !string.IsNullOrEmpty(model.sortOrder))
            {
                sortField = model.sortField; sortOrder = model.sortOrder;
            }
            var filteredSubmission = allSubmission.Select(x => new
            {
                x.Id,
                x.Title,
                x.Keywords,
                x.UpdatedDate,
                x.SubmissionStatus,
                x.CreatedDate,
            });
            filteredSubmission = filteredSubmission.OrderBy(sortField, sortOrder == "asc");
            var submissions = filteredSubmission.Skip((model.pageIndex - 1) * model.pageSize).Take(model.pageSize);
            return new SubmissionGridModel
            {
                Data = submissions.Select(x => new SubmissionGridRowModel
                {
                    Keywords = x.Keywords,
                    LastActivityDate = x.UpdatedDate.ToString("dd MMM yyyy"),
                    SubmissionDate = x.CreatedDate.ToString("dd MMM yyyy"),
                    Title = x.Title,
                    SubmissionId = x.Id,
                    Status = x.SubmissionStatus.ToString()
                }).ToList(),
                ItemsCount = allSubmission.Count()
            };
        }
        public AssignedSubmissionCount SubmissionCount(string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var statuses = new SubmissionStatus[] { SubmissionStatus.Submission, SubmissionStatus.Review };
            var assigned = context.Submission.Count(x => x.EditorId != null && statuses.Contains(x.SubmissionStatus) && x.User.Tenant.JournalPath == journalPath);
            var unAssigned = context.Submission.Count(x => x.EditorId == null && statuses.Contains(x.SubmissionStatus) && x.User.Tenant.JournalPath == journalPath);
            return new AssignedSubmissionCount { Assigned = assigned, UnAssigned = unAssigned };
        }
        public EICSubmissionGridModel JournalSubmission(string journalPath, EditorSubmissionGridSearchModel model, long? assignerId = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var statuses = new SubmissionStatus[] { SubmissionStatus.Submission, SubmissionStatus.Review };
            var dbSubmission = context.Submission.Where(x => x.User.Tenant.JournalPath == journalPath && statuses.Contains(x.SubmissionStatus));
            if (assignerId.HasValue)
            {
                dbSubmission = dbSubmission.Where(x => x.EditorId == assignerId);
            }
            if (model.AssignedStatus == EditorAssignedStatus.UnAssigned)
            {
                dbSubmission = dbSubmission.Where(x => x.EditorId == null);
            }
            else if (model.AssignedStatus == EditorAssignedStatus.Assigned)
            {
                dbSubmission = dbSubmission.Where(x => x.EditorId != null);
                if (model.EditerId.HasValue)
                {
                    dbSubmission = dbSubmission.Where(x => x.EditorId == model.EditerId.Value);
                }
            }
            var submissions = dbSubmission.Select(x => new
            {
                x.User.FirstName,
                x.User.LastName,
                x.UpdatedDate,
                x.Prefix,
                x.SubmissionStatus,
                SubmissionID = x.Id,
                x.Subtitle,
                x.Title
            });
            if (!string.IsNullOrEmpty(model.SrchText))
            {
                submissions = submissions.Where(x => EF.Functions.ILike(x.Subtitle, $"%{model.SrchText}%") || EF.Functions.ILike(x.Title, $"%{model.SrchText}%") || EF.Functions.ILike(x.Prefix, $"%{model.SrchText}%"));
            }
            if (!string.IsNullOrEmpty(model.Author))
            {
                submissions = submissions.Where(x => EF.Functions.ILike(x.FirstName + " " + x.LastName, $"%{model.Author}%"));
            }
            if (model.SubmissionStatus.HasValue)
            {
                submissions = submissions.Where(x => x.SubmissionStatus == model.SubmissionStatus);
            }
            if (string.IsNullOrEmpty(model.sortOrder) || model.sortOrder == "desc")
            {
                submissions = submissions.OrderByDescending(x => x.UpdatedDate);
            }
            else
            {
                submissions = submissions.OrderBy(x => x.UpdatedDate);
            }
            return new EICSubmissionGridModel
            {
                ItemsCount = submissions.Count(),
                Data = submissions.Skip((model.pageIndex - 1) * model.pageSize).Take(model.pageSize).ToList().Select(x => new EICSubmissionGridModelItem
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    LastUpdated = x.UpdatedDate.ToString("dd MMM yyyy"),
                    Prefix = x.Prefix,
                    Status = x.SubmissionStatus.ToString(),
                    SubmissionID = x.SubmissionID,
                    SubTitle = x.Subtitle,
                    Title = x.Title
                })
            };


        }

        public EditorSubmissionViewModel GetEditorSubmissionViewModel(long submissionId, string journalPath = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == submissionId && x.SubmissionStatus != SubmissionStatus.Draft);
            if (!string.IsNullOrEmpty(journalPath))
            {
                submissions = submissions.Where(x => x.User.Tenant.JournalPath == journalPath);
            }
            var submission = submissions.First();
            var model = new EditorSubmissionViewModel
            {
                Abstract = submission.Abstract,
                Comments = submission.EditorComment,
                SubmissionId = submission.Id,
                EditorId = submission.EditorId,
                Keywords = submission.Keywords,
                Prefix = submission.Prefix,
                SubmissionStatus = submission.SubmissionStatus.ToString(),
                SubTitle = submission.Subtitle,
                Title = submission.Title,
                Files = context.SubmisssionFile.Where(x => x.SubmissionId == submission.Id).Select(x => new { x.TenantArticleComponent.Text, x.FileName, x.UploadedOn, x.Id }).ToList().Select(x => new SubmissionFileListModel
                {
                    SubmissionFileID = x.Id,
                    ArticalComponent = x.Text,
                    FileName = x.FileName,
                    UploadDate = x.UploadedOn.ToString("dd MMM yyyy")
                }).ToList()
            };
            return model;
        }
        public AuthorSubmissionViewModel GetAuthorSubmissionViewModel(long submissionId,long? userID=null, string journalPath = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == submissionId && x.SubmissionStatus != SubmissionStatus.Draft);
            if (!string.IsNullOrEmpty(journalPath))
            {
                submissions = submissions.Where(x => x.User.Tenant.JournalPath == journalPath);
            }
            if (userID.HasValue)
            {
                submissions = submissions.Where(x => x.UserID == userID.Value);
            }
            var submission = submissions.First();
            var model = new AuthorSubmissionViewModel
            {
                Abstract = submission.Abstract,
                Comments = submission.EditorComment,
                SubmissionId = submission.Id,
                Keywords = submission.Keywords,
                Prefix = submission.Prefix,
                SubmissionStatus = submission.SubmissionStatus.ToString(),
                SubTitle = submission.Subtitle,
                Title = submission.Title,
                Files = context.SubmisssionFile.Where(x => x.SubmissionId == submission.Id).Select(x => new { x.TenantArticleComponent.Text, x.FileName, x.UploadedOn, x.Id }).ToList().Select(x => new SubmissionFileListModel
                {
                    SubmissionFileID = x.Id,
                    ArticalComponent = x.Text,
                    FileName = x.FileName,
                    UploadDate = x.UploadedOn.ToString("dd MMM yyyy")
                }).ToList(),
                Contributers= context.Contributors.Where(x => x.SubmissionId == submission.Id).Select(x => new { x.FirstName, x.LastName, x.Email, x.ContributerRole }).ToList().Select(x => new ContributerListModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Role = x.ContributerRole.ToString()
                }).ToList(),
            };
            return model;
        }
        public void AssignEditor(long submissionId, long? editorId, string journalPath = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == submissionId && x.SubmissionStatus != SubmissionStatus.Draft);
            if (!string.IsNullOrEmpty(journalPath))
            {
                submissions = submissions.Where(x => x.User.Tenant.JournalPath == journalPath);
            }
            var submission = submissions.First();
            submission.EditorId = editorId;
            context.SaveChanges();
        }

        public SubmisssionFile GetSubmissionFile(long submissionFileId, string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissionFile = context.SubmisssionFile.Where(x => x.Id == submissionFileId && x.Submission.User.Tenant.JournalPath == journalPath).First();
            return submissionFile;
        }

        public SubmissionFileDetailsViewModel SubmisssionFileDetails(long submissionFileId, string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissionFile = context.SubmisssionFile.IncludeMultiple(x=>x.TenantArticleComponent).Where(x => x.Id == submissionFileId && x.Submission.User.Tenant.JournalPath == journalPath).First();
            var model = new SubmissionFileDetailsViewModel
            {
                ArticleComponent = submissionFile.TenantArticleComponent.Text,
                Creator = submissionFile.Creator,
                Description = submissionFile.Description,
                FileName = submissionFile.FileName,
                Subject = submissionFile.Subject,
                UploadedOn = submissionFile.UploadedOn.ToString("dd MMM yyyy")
            };
            return model;
        } 

        public void MoveToReview(long submissionId, string journalPath = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == submissionId);
            if (!string.IsNullOrEmpty(journalPath))
            {
                submissions = submissions.Where(x => x.User.Tenant.JournalPath == journalPath);
            }
            submissions.Update(x => new Submission { SubmissionStatus = SubmissionStatus.Review });

        }

        public void RejectSubmission(long submissionID,string journalPath)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var submissions = context.Submission.Where(x => x.Id == submissionID && x.User.Tenant.JournalPath == journalPath);
             submissions.Update(x => new Submission { SubmissionStatus = SubmissionStatus.Rejected });
        }

        public RejectedSubmissionGridModel GetRejectedSubmissions(string journalPath, RejectedSubmissionGridSearchModel model, long? editerID = null)
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var statuses = new SubmissionStatus[] { SubmissionStatus.Submission, SubmissionStatus.Review };
            var dbSubmission = context.Submission.Where(x => x.User.Tenant.JournalPath == journalPath && x.SubmissionStatus== SubmissionStatus.Rejected);
            if (editerID.HasValue)
            {
                dbSubmission = dbSubmission.Where(x => x.EditorId == editerID);
            }
            var submissions = dbSubmission.Select(x => new
            {
                x.User.FirstName,
                x.User.LastName,
                x.UpdatedDate,
                x.Prefix,
                x.SubmissionStatus,
                SubmissionID = x.Id,
                x.Subtitle,
                x.Title
            });
            if (!string.IsNullOrEmpty(model.SrchText))
            {
                submissions = submissions.Where(x => EF.Functions.ILike(x.Subtitle, $"%{model.SrchText}%") || EF.Functions.ILike(x.Title, $"%{model.SrchText}%") || EF.Functions.ILike(x.Prefix, $"%{model.SrchText}%"));
            }
            if (!string.IsNullOrEmpty(model.Author))
            {
                submissions = submissions.Where(x => EF.Functions.ILike(x.FirstName + " " + x.LastName, $"%{model.Author}%"));
            }
            
            if (string.IsNullOrEmpty(model.sortOrder) || model.sortOrder == "desc")
            {
                submissions = submissions.OrderByDescending(x => x.UpdatedDate);
            }
            else
            {
                submissions = submissions.OrderBy(x => x.UpdatedDate);
            }
            return new RejectedSubmissionGridModel
            {
                ItemsCount = submissions.Count(),
                Data = submissions.Skip((model.pageIndex - 1) * model.pageSize).Take(model.pageSize).ToList().Select(x => new RejectedSubmissionGridRowModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    LastUpdated = x.UpdatedDate.ToString("dd MMM yyyy"),
                    Prefix = x.Prefix,
                    SubmissionID = x.SubmissionID,
                    SubTitle = x.Subtitle,
                    Title = x.Title
                })
            };
        }
    }
}
