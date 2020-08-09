using JMS.Entity.Entities;
using JMS.ViewModels.Submissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ISubmissionService
    {
        long CreateSubmission(CreateSubmissionModel createSubmissionModel,long userId);
        SubmissionFileListModel AddSubmissionFile(AddSubmissionFileModel model);
        Dictionary<long, string> GetArticleComponent(string journalPath);
        Submission GetSubmission(long SubmissionId, long userId);
        IEnumerable<SubmissionFileListModel> GetSubmissionFiles(long submissionId, long userId);
        SubmisssionFile GetSubmissionFile(long submissionFileId, long userId);
        void SaveSubmissionFile(EditSubmissionFileModel model, long userId);
        void RemoveFile(long submissionFileId, long userId);
        void EditSubmission(EditSubmissionMetadataModel model, long userId);
        void MovetoContributer(long submissionId, long userId);
        bool ValidateContributerEmail(string email, long submissionID, ApplicationUser user, long? contributerId);
        void AddContributer(AddContributerViewModel contibuterViewModel);
        IEnumerable<ContributerListModel> Contributers(long submissionId, long? userID);
        Contributor GetContributor(long contributerId, long? userID);
        void EditContributer(EditContributerModel model, long? userId);
        void DeleteContributor(long contributerId, long? userID);
        void MovetoFinish(long submissionId, long userId);
        void EditorComment(EditorCommentModel model, long? userID);
        SubmissionGridModel GetSubmissions(long userID, SubmissionGridSearchModel model);
        EICSubmissionGridModel JournalSubmission(string journalPath, EditorSubmissionGridSearchModel model);

        AssignedSubmissionCount SubmissionCount(string journalPath);
    }
}
