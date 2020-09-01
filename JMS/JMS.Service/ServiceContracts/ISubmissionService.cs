using JMS.Entity.Entities;
using JMS.ViewModels.Submissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ISubmissionService
    {
        long CreateSubmission(CreateSubmissionModel createSubmissionModel, long userId);
        SubmissionFileListModel AddSubmissionFile(AddSubmissionFileModel model);
        Dictionary<long, string> GetArticleComponent(string journalPath);
        Submission GetSubmission(long SubmissionId, long? userId = null, string journalPath = null);
        IEnumerable<SubmissionFileListModel> GetSubmissionFiles(long submissionId, long userId);
        SubmisssionFile GetSubmissionFile(long submissionFileId, long userId);
        SubmisssionFile GetSubmissionFile(long submissionFileId, string journalPath);
        void SaveSubmissionFile(EditSubmissionFileModel model, long userId);
        void RemoveFile(SubmisssionFile file, long userId);
        void EditSubmission(EditSubmissionMetadataModel model, long userId);
        void MovetoContributer(long submissionId, long userId);
        bool ValidateContributerEmail(string email, long submissionID, ApplicationUser user, long? contributerId);
        void AddContributer(AddContributerViewModel contibuterViewModel);
        IEnumerable<ContributerListModel> Contributers(long submissionId, long? userID);
        Contributor GetContributor(long contributerId, long? userID);
        void EditContributer(EditContributerModel model, long? userId);
        void DeleteContributor(Contributor contributor);
        void MovetoFinish(long submissionId, long userId);
        void EditorComment(Submission submission, EditorCommentModel model, long userID, string path = null);
        SubmissionGridModel GetSubmissions(long userID, SubmissionGridSearchModel model);
        EICSubmissionGridModel JournalSubmission(string journalPath, EditorSubmissionGridSearchModel model, long? assignerId = null);

        AssignedSubmissionCount SubmissionCount(string journalPath);
        EditorSubmissionViewModel GetEditorSubmissionViewModel(long submissionId, string journalPath = null);
        void AssignEditor(long submissionId, long? editorId, string journalPath = null);
        SubmissionFileDetailsViewModel SubmisssionFileDetails(long submissionFileId, string journalPath);
        void RemoveSubmission(long submissionId, long userId);
        void RemoveSubmission(long submissionId, string path);
        void MoveToReview(long submissionId, string journalPath = null);
        void RejectSubmission(RejectSubmission rejectSubmission, string journalPath);
        RejectedSubmissionGridModel GetRejectedSubmissions(string journalPath, RejectedSubmissionGridSearchModel model, long? editerID = null);
        AuthorSubmissionViewModel GetAuthorSubmissionViewModel(long submissionId, long? userID = null, string journalPath = null);

        void SaveSubmissionHistory(SubmissionHistory history);
        IEnumerable<ActivityLog> GetActivityLogs(long submissionId, long tenantID);
    }
}
