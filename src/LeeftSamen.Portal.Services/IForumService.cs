// <copyright file="IForumService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Services
{
    public interface IForumService
    {
        Task AddChildReactionAsync(User user, ForumReaction parent, string text);

        Task AddReactionAsync(User user, ForumSubject subject, string title, string text, List<Media> files, bool isMain);

        Task<ForumSubject> AddSubjectAsync(User user, string type, int typeId, string title, string description);

        Task EditReactionAsync(ForumReaction reaction, string text, List<Media> files, List<int> oldFiles, User editor);

        Task EditSubjectAsync(ForumSubject subject, string title, string description);

        Task<List<ForumReactionReport>> GetActiveReactionReportsAsync(string type, int typeId);

        Task<ForumReaction> GetLastReactionBySubjectIdAsync(int subjectId);

        Task<ForumReaction> GetReactionByIdAsync(int? reactionId);

        Task<int> GetReactionCountBySubjectIdAsync(int subjectId);

        Task<int> GetReactionCountWithChildsBySubjectIdAsync(int subjectId);

        Task<int> GetReactionCountByUserAsync(User user);

        Task<List<ForumReactionReport>> GetReactionReportsAsync(int reactionId);

        Task<ForumReactionReport> GetReportedReactionAsync(int? reportId);

        Task<ForumSubject> GetSubjectByIdAsync(int? subjectId);

        Task<List<ForumSubject>> GetSubjectsAsync(string type, int typeId);

        Task<List<ForumReaction>> GetReactionsByParentReactionIdAsync(int reactionId);

        Task<List<ForumReaction>> GetReactionsBySubjectAsync(ForumSubject subject, int pageSize, int page);

        Task IgnoreReportedReactionAsync(ForumReactionReport report, User ignoredBy);

        Task<bool> IsReactionReportedAsync(int? reactionId, User user);

        Task RemoveReactionAsync(ForumReaction reaction, User deletedBy);

        Task RemoveSubject(ForumSubject subject);

        Task ReportReactionAsync(ForumReaction reaction, User reportedBy);
    }
}
