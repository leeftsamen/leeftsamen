// <copyright file="ForumService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data;
using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.Data.Models;
using LeeftSamen.Portal.EmailTemplates.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Services
{
    class ForumService : IForumService
    {
        private readonly IApplicationDbContext databaseContext;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMailerService mailerService;

        private readonly INotificationService notificationService;

        public ForumService(
            IApplicationDbContext databaseContext,
            ILinkGenerator linkGenerator,
            INotificationService notificationService,
            IMailerService mailerService)
        {
            this.databaseContext = databaseContext;
            this.mailerService = mailerService;
            this.linkGenerator = linkGenerator;
            this.notificationService = notificationService;
        }

        public IQueryable<ForumReaction> ActiveReactions()
        {
            return this.databaseContext.ForumReactions.Where(r => !r.Deleted && (r.ParentReaction == null || !r.ParentReaction.Deleted));
        }

        public async Task AddChildReactionAsync(User user, ForumReaction parent, string text)
        {
            var reaction = this.databaseContext.ForumReactions.Create();
            reaction.CreationDate = DateTime.Now;
            reaction.Creator = user;
            reaction.Message = text;
            reaction.ParentReaction = parent;

            this.databaseContext.ForumReactions.Add(reaction);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddReactionAsync(User user, ForumSubject subject, string title, string text, List<Media> files, bool isMain)
        {
            var reaction = this.databaseContext.ForumReactions.Create();
            reaction.CreationDate = DateTime.Now;
            reaction.Creator = user;
            reaction.Subject = subject;
            reaction.SubjectMainReaction = isMain;
            reaction.Title = title;
            reaction.Message = text;
            reaction.MediaList = files.Select(f => new ForumReactionMedia() { Media = f, Reaction = reaction }).ToList();

            this.databaseContext.ForumReactions.Add(reaction);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ForumSubject> AddSubjectAsync(User user, string type, int typeId, string title, string description)
        {
            var item = this.databaseContext.ForumSubjects.Create();
            item.Creator = user;
            item.Type = type;
            item.TypeId = typeId;
            item.Title = title;
            item.Description = description;
            item.CreationDate = DateTime.Now;

            this.databaseContext.ForumSubjects.Add(item);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return item;
        }

        public async Task EditReactionAsync(ForumReaction reaction, string text, List<Media> files, List<int> oldFiles, User editor)
        {
            reaction.Message = text;
            reaction.EditCount++;
            reaction.LastEditBy = editor;
            reaction.LastEditDate = DateTime.Now;

            var filesToRemove = new List<ForumReactionMedia>();
            foreach (var reactionMedia in reaction.MediaList)
            {
                if (oldFiles.All(f => f != reactionMedia.Media.MediaId))
                {
                    filesToRemove.Add(reactionMedia);
                }
            }

            foreach (var reactionMedia in filesToRemove)
            {
                this.databaseContext.Media.Remove(reactionMedia.Media);
                this.databaseContext.ForumReactionMediaList.Remove(reactionMedia);
            }

            reaction.MediaList.AddRange(files.Select(f => new ForumReactionMedia() { Media = f, Reaction = reaction }));

            if (editor.Id != reaction.Creator.Id)
            {
                var reports = await this.GetReactionReportsAsync(reaction.ForumReactionId).ConfigureAwait(false);
                foreach (var report in reports)
                {
                    report.Reviewed = true;
                    report.ReviewedBy = editor;
                }
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EditSubjectAsync(ForumSubject subject, string title, string description)
        {
            subject.Title = title;
            subject.Description = description;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<ForumReactionReport>> GetActiveReactionReportsAsync(string type, int typeId)
        {
            return
                await
                    this.databaseContext.ForumReactionReports.Include(r => r.Reaction).Include(r => r.Reporter).Include(r => r.Reaction.Creator).Include(r => r.Reaction.MediaList.Select(m => m.Media)).Where(
                        s => !s.Reviewed && ((s.Reaction.Subject.Type == type && s.Reaction.Subject.TypeId == typeId) || (s.Reaction.ParentReaction.Subject.Type == type && s.Reaction.ParentReaction.Subject.TypeId == typeId)))
                        .ToListAsync()
                        .ConfigureAwait(false);
        }

        public async Task<ForumReaction> GetLastReactionBySubjectIdAsync(int subjectId)
        {
            return
                await
                    this.ActiveReactions().Include(s => s.Creator).Where(
                        s => s.Subject.ForumSubjectId == subjectId || (s.ParentReaction != null && s.ParentReaction.Subject.ForumSubjectId == subjectId))
                        .OrderByDescending(s => s.ForumReactionId)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
        }

        public async Task<ForumReaction> GetReactionByIdAsync(int? reactionId)
        {
            return
                await
                this.ActiveReactions().Include(r => r.MediaList.Select(m => m.Media)).FirstOrDefaultAsync(c => c.ForumReactionId == reactionId).ConfigureAwait(false);
        }

        public async Task<int> GetReactionCountBySubjectIdAsync(int subjectId)
        {
            return
                await
                    this.ActiveReactions().CountAsync(
                        s => s.Subject.ForumSubjectId == subjectId).ConfigureAwait(false);
        }

        public async Task<int> GetReactionCountWithChildsBySubjectIdAsync(int subjectId)
        {
            return
                await
                    this.ActiveReactions().CountAsync(
                        s => s.Subject.ForumSubjectId == subjectId || (s.ParentReaction != null && s.ParentReaction.Subject.ForumSubjectId == subjectId)).ConfigureAwait(false);
        }

        public async Task<int> GetReactionCountByUserAsync(User user)
        {
            return
                await
                    this.ActiveReactions().CountAsync(
                        s => s.Creator.Id == user.Id).ConfigureAwait(false);
        }

        public async Task<List<ForumReactionReport>> GetReactionReportsAsync(int reactionId)
        {
            return
                await
                    this.databaseContext.ForumReactionReports.Where(
                        s => s.Reaction.ForumReactionId == reactionId)
                        .ToListAsync()
                        .ConfigureAwait(false);
        }

        public async Task<ForumReactionReport> GetReportedReactionAsync(int? reportId)
        {
            return await this.databaseContext.ForumReactionReports.FirstOrDefaultAsync(r => r.ForumReactionReportId == reportId).ConfigureAwait(false);
        }

        public async Task<ForumSubject> GetSubjectByIdAsync(int? subjectId)
        {
            return
                await
                this.databaseContext.ForumSubjects.Where(s => !s.Deleted).FirstOrDefaultAsync(c => c.ForumSubjectId == subjectId).ConfigureAwait(false);
        }

        public async Task<List<ForumSubject>> GetSubjectsAsync(string type, int typeId)
        {
            return
                await
                this.databaseContext.ForumSubjects.Where(c => c.Type == type && c.TypeId == typeId && !c.Deleted).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<ForumReaction>> GetReactionsByParentReactionIdAsync(int reactionId)
        {
            return
                await
                    this.ActiveReactions().Include(s => s.Creator).Include(r => r.MediaList.Select(m => m.Media)).Where(
                        r => r.ParentReaction.ForumReactionId == reactionId).OrderBy(s => s.ForumReactionId)
                        .ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<ForumReaction>> GetReactionsBySubjectAsync(ForumSubject subject, int pageSize, int page)
        {
            return
                await
                    this.ActiveReactions().Include(s => s.Creator).Include(r => r.MediaList.Select(m => m.Media)).Where(
                        s => s.Subject.ForumSubjectId == subject.ForumSubjectId).OrderByDescending(s => s.ForumReactionId)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);
        }

        public async Task IgnoreReportedReactionAsync(ForumReactionReport report, User ignoredBy)
        {
            report.Reviewed = true;
            report.ReviewedBy = ignoredBy;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> IsReactionReportedAsync(int? reactionId, User user)
        {
            return await this.databaseContext.ForumReactionReports.AnyAsync(r => r.Reaction.ForumReactionId == reactionId && r.Reporter.Id == user.Id && !r.Reviewed).ConfigureAwait(false);
        }

        public async Task RemoveReactionAsync(ForumReaction reaction, User deletedBy)
        {
            var childReactions = await this.GetReactionsByParentReactionIdAsync(reaction.ForumReactionId).ConfigureAwait(false);
            reaction.Deleted = true;
            reaction.DeletedBy = deletedBy;
            reaction.DeletedDate = DateTime.Now;

            var reports = await this.GetReactionReportsAsync(reaction.ForumReactionId).ConfigureAwait(false);
            foreach (var report in reports)
            {
                report.Reviewed = true;
                report.ReviewedBy = deletedBy;
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            if (reaction.Creator.Id != deletedBy.Id)
            {
                await this.SendRemoveReactionEmailAsync(reaction, deletedBy).ConfigureAwait(false);
                await this.SendNotificationRemoveReactionAsync(reaction, deletedBy).ConfigureAwait(false);
            }

            HashSet<string> userIds = new HashSet<string>();
            userIds.Add(reaction.Creator.Id);
            foreach (var user in childReactions.Select(r => r.Creator))
            {
                if (!userIds.Contains(user.Id))
                {
                    await this.SendRemoveParentReactionEmailAsync(user, reaction, deletedBy).ConfigureAwait(false);
                    await this.SendNotificationRemoveParentReactionAsync(user, reaction, deletedBy).ConfigureAwait(false);
                    userIds.Add(user.Id);
                }
            }
        }

        public async Task RemoveSubject(ForumSubject subject)
        {
            subject.Deleted = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ReportReactionAsync(ForumReaction reaction, User reportedBy)
        {
            var report = this.databaseContext.ForumReactionReports.Create();
            report.Reaction = reaction;
            report.Reporter = reportedBy;
            report.ReportDate = DateTime.Now;
            this.databaseContext.ForumReactionReports.Add(report);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task SendRemoveReactionEmailAsync(ForumReaction reaction, User deletedBy)
        {
            var model = new RemoveReactionModel
            {
                Subject = Subject.RemoveReaction,
                Name = reaction.Creator.Name,
                ReactionDate = reaction.CreationDate,
                ReactionText = reaction.Message,
                RemovedByName = deletedBy.Name,
                RemovedDate = DateTime.Now
            };

            await this.mailerService.SendAsync(model, reaction.Creator).ConfigureAwait(false);
        }

        private async Task<bool> SendNotificationRemoveReactionAsync(ForumReaction reaction, User deletedBy)
        {
            var subject = (reaction.ParentReaction ?? reaction).Subject;
            var link = this.linkGenerator.GenerateRemovedReactionLink(subject.ForumSubjectId, subject.Type, subject.TypeId);
            var message = string.Format(Common.InterfaceText.Notification.RemoveReaction, subject.Title);

            //await this.notificationService.CreateNotificationForUserAsync(reaction.Creator, message, link, SettingName.RemoveReaction).ConfigureAwait(false);

            return true;
        }

        private async Task SendRemoveParentReactionEmailAsync(User owner, ForumReaction parentReaction, User deletedBy)
        {
            var model = new RemoveParentReactionModel
            {
                Subject = Subject.RemoveReaction,
                Name = owner.Name,
                ParentReactionText = parentReaction.Message,
                RemovedByName = deletedBy.Name,
                RemovedDate = DateTime.Now
            };

            await this.mailerService.SendAsync(model, owner).ConfigureAwait(false);
        }

        private async Task<bool> SendNotificationRemoveParentReactionAsync(User owner, ForumReaction parentReaction, User deletedBy)
        {
            var subject = parentReaction.Subject;
            var link = this.linkGenerator.GenerateRemovedReactionLink(subject.ForumSubjectId, subject.Type, subject.TypeId);
            var message = string.Format(Common.InterfaceText.Notification.RemoveParentReaction, subject.Title);

            //await this.notificationService.CreateNotificationForUserAsync(owner, message, link, SettingName.RemoveParentReaction).ConfigureAwait(false);

            return true;
        }
    }
}
