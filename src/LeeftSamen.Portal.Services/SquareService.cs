// <copyright file="SquareService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Web;
using LeeftSamen.Portal.Services.DTO;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;

    public class SquareService : ISquareService
    {
        private readonly int[] coverColors;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMediaService mediaService;

        private readonly IApplicationDbContext databaseContext;

        private IQueryable<Square> UserSquares(User user)
        {
            return this.databaseContext.Squares.Where(s => s.ZipCodes.Any(z => z.ZipCode == user.PostalCode) || s.Admins.Any(a => a.User.Id == user.Id));
        }

        public SquareService(IApplicationDbContext databaseContext,
            IMediaService mediaService,
            ILinkGenerator linkGenerator)
        {
            this.databaseContext = databaseContext;
            this.coverColors = new[] { 0xf5736e, 0x2ec5c2, 0x7e66a5, 0xfecb50 };
            this.mediaService = mediaService;
            this.linkGenerator = linkGenerator;
        }

        public int[] GetCoverColors()
        {
            return this.coverColors;
        }

        public bool IsUserSquareAdmin(int? squareId, User user)
        {
            return this.UserSquares(user).Any(s => s.Admins.Any(a => a.User.Id == user.Id));
        }

        public async Task<List<Square>> GetUserSquaresAsync(User user)
        {
            return await this.UserSquares(user).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Square> GetSquareByIdAsync(int? squareId, User user)
        {
            return
                await
                this.UserSquares(user).FirstOrDefaultAsync(c => c.SquareId == squareId).ConfigureAwait(false);
        }

        public async Task AddSquareZipCodeAsync(Square square, string zipcode)
        {
            var item = this.databaseContext.SquareZipCodes.Create();
            item.Square = square;
            item.ZipCode = zipcode;
            this.databaseContext.SquareZipCodes.Add(item);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveSquareZipCodeAsync(Square square, int zipcodeId)
        {
            var squareZipCode = await
                this.databaseContext.SquareZipCodes.FirstOrDefaultAsync(
                    z => z.SquareZipCodeId == zipcodeId && z.Square.SquareId == square.SquareId).ConfigureAwait(false);
            if (squareZipCode != null)
            {
                this.databaseContext.SquareZipCodes.Remove(squareZipCode);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<SquareZipCode>> GetSquareZipCodesAsync(Square square)
        {
            return await this.databaseContext.SquareZipCodes.Where(z => z.Square.SquareId == square.SquareId).OrderBy(z => z.ZipCode).ToListAsync().ConfigureAwait(false);
        }

        public async Task<bool> IsUserSquareAdminAsync(int? squareId, User user)
        {
            return await this.UserSquares(user).AnyAsync(s => s.SquareId == squareId && s.Admins.Any(a => a.User.Id == user.Id)).ConfigureAwait(false);
        }

        public async Task<List<User>> GetSquareAdminsAsync(int? squareId)
        {
            return
                await
                    this.databaseContext.SquareAdmins.Where(a => a.Square.SquareId == squareId)
                        .Select(a => a.User)
                        .ToListAsync()
                        .ConfigureAwait(false);
        }

        public async Task RemoveAdminAsync(int? squareId, string userId)
        {
            var squareAdmin = await
                this.databaseContext.SquareAdmins.FirstOrDefaultAsync(
                    a => a.Square.SquareId == squareId && a.User.Id == userId).ConfigureAwait(false);
            if (squareAdmin != null)
            {
                this.databaseContext.SquareAdmins.Remove(squareAdmin);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<SquareFact> GetSquareFactByIdAsync(int? factId)
        {
            return
                await
                this.databaseContext.SquareFacts.Include(s => s.MediaList.Select(m => m.Media)).FirstOrDefaultAsync(c => c.SquareFactId == factId).ConfigureAwait(false);
        }

        public async Task<List<SquareFact>> GetSquareFactsAsync(Square square)
        {
            return
                await
                this.databaseContext.SquareFacts.Include(s => s.MediaList.Select(m => m.Media)).Where(c => c.Square.SquareId == square.SquareId).ToListAsync().ConfigureAwait(false);
        }

        public async Task AddSquareFactAsync(User user, Square square, string title, string introductiontext, string fullText, List<Media> files)
        {
            var item = this.databaseContext.SquareFacts.Create();
            item.Creator = user;
            item.Square = square;
            item.Title = title;
            item.IntroductionText = introductiontext;
            item.FullText = fullText;
            item.CreationDate = DateTime.Now;
            item.MediaList = files.Select(f => new SquareFactMedia() {Media = f, SquareFact = item}).ToList();

            this.databaseContext.SquareFacts.Add(item);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EditSquareFactAsync(SquareFact squareFact, string title, string introductiontext, string fullText, List<Media> files, List<int> oldFiles)
        {
            squareFact.Title = title;
            squareFact.IntroductionText = introductiontext;
            squareFact.FullText = fullText;
            squareFact.CreationDate = DateTime.Now;

            var filesToRemove = new List<SquareFactMedia>();
            foreach (var factMedia in squareFact.MediaList)
            {
                if (oldFiles.All(f => f != factMedia.Media.MediaId))
                {
                    filesToRemove.Add(factMedia);
                }
            }

            foreach (var factMedia in filesToRemove)
            {
                this.databaseContext.Media.Remove(factMedia.Media);
                this.databaseContext.SquareFactMediaList.Remove(factMedia);
            }

            squareFact.MediaList.AddRange(files.Select(f => new SquareFactMedia() { Media = f, SquareFact = squareFact }));

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveSquareFactAsync(SquareFact fact)
        {
            var files = this.databaseContext.SquareFactMediaList.Where(m => m.SquareFact.SquareFactId == fact.SquareFactId).ToList();
            foreach (var file in files)
            {
                this.databaseContext.SquareFactMediaList.Remove(file);
            }

            this.databaseContext.SquareFacts.Remove(fact);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        //public async Task AddSquareSubjectAsync(User user, Square square, string title, string description)
        //{
        //    var item = this.databaseContext.SquareForumSubjects.Create();
        //    item.Creator = user;
        //    item.Square = square;
        //    item.Title = title;
        //    item.Description = description;
        //    item.CreationDate = DateTime.Now;

        //    this.databaseContext.SquareForumSubjects.Add(item);
        //    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<ForumSubject> GetSquareForumSubjectByIdAsync(int? subjectId)
        //{
        //    return
        //        await
        //        this.databaseContext.SquareForumSubjects.FirstOrDefaultAsync(c => c.SquareForumSubjectId == subjectId).ConfigureAwait(false);
        //}

        //public async Task<List<ForumSubject>> GetSquareForumSubjectsAsync(Square square)
        //{
        //    return
        //        await
        //        this.databaseContext.SquareForumSubjects.Where(c => c.Square.SquareId == square.SquareId).ToListAsync().ConfigureAwait(false);
        //}

        //public async Task<List<ForumReaction>> GetSquareForumSubjectReactionsBySubjectAsync(ForumSubject subject, int pageSize, int page = 1)
        //{
        //    return
        //        await
        //            this.databaseContext.SquareForumSubjectReactions.Include(s => s.Creator).Include(r => r.MediaList.Select(m => m.Media)).Where(
        //                s => s.Subject.SquareForumSubjectId == subject.SquareForumSubjectId).OrderByDescending(s => s.SquareForumSubjectReactionId)
        //                .Skip((page - 1)* pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);
        //}

        //public async Task<int> GetForumReactionCountBySubjectAsync(int subjectId)
        //{
        //    return
        //        await
        //            this.databaseContext.SquareForumSubjectReactions.CountAsync(
        //                s => s.Subject.SquareForumSubjectId == subjectId).ConfigureAwait(false);
        //}

        //public async Task<ForumReaction> GetForumLastReactionBySubjectAsync(int subjectId)
        //{
        //    return
        //        await
        //            this.databaseContext.SquareForumSubjectReactions.Include(s => s.Creator).Where(
        //                s => s.Subject.SquareForumSubjectId == subjectId)
        //                .OrderByDescending(s => s.SquareForumSubjectReactionId)
        //                .FirstOrDefaultAsync()
        //                .ConfigureAwait(false);
        //}

        //public async Task<List<ForumReactionReport>> GetReactionReportsAsync(int reactionId)
        //{
        //    return
        //        await
        //            this.databaseContext.ForumReactionReports.Where(
        //                s => s.Reaction.ForumReactionId == reactionId)
        //                .ToListAsync()
        //                .ConfigureAwait(false);
        //}

        //public async Task<List<ForumReactionReport>> GetActiveReactionReportsBySquareIdAsync(int squareId)
        //{
        //    return
        //        await
        //            this.databaseContext.ForumReactionReports.Include(r => r.Reaction).Include(r => r.Reporter).Include(r => r.Reaction.Creator).Include(r => r.Reaction.MediaList.Select(m => m.Media)).Where(
        //                s => !s.Reviewed && s.Reaction.Subject.Square.SquareId == squareId)
        //                .ToListAsync()
        //                .ConfigureAwait(false);
        //}

        //public async Task AddSquareForumReactionAsync(User user, ForumSubject subject, string text, List<Media> files)
        //{
        //    var reaction = new ForumReaction();
        //    reaction.CreationDate = DateTime.Now;
        //    reaction.Creator = user;
        //    reaction.Subject = subject;
        //    reaction.Message = text;
        //    reaction.MediaList = files.Select(f => new ForumReactionMedia() { Media = f, Reaction = reaction }).ToList();

        //    this.databaseContext.SquareForumSubjectReactions.Add(reaction);
        //    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task EditSquareForumReactionAsync(ForumReaction reaction, string text, List<Media> files, List<int> oldFiles, User editor)
        //{
        //    reaction.Message = text;
        //    reaction.EditCount++;
        //    reaction.LastEditBy = editor;
        //    reaction.LastEditDate = DateTime.Now;

        //    var filesToRemove = new List<ForumReactionMedia>();
        //    foreach (var reactionMedia in reaction.MediaList)
        //    {
        //        if (oldFiles.All(f => f != reactionMedia.Media.MediaId))
        //        {
        //            filesToRemove.Add(reactionMedia);
        //        }
        //    }
        //    foreach (var reactionMedia in filesToRemove)
        //    {
        //        this.databaseContext.Media.Remove(reactionMedia.Media);
        //        this.databaseContext.SquareForumReactionMediaList.Remove(reactionMedia);
        //    }

        //    reaction.MediaList.AddRange(files.Select(f => new ForumReactionMedia() { Media = f, Reaction = reaction }));

        //    if (editor.Id != reaction.Creator.Id)
        //    {
        //        var reports = await this.GetReactionReportsAsync(reaction.ForumReactionId).ConfigureAwait(false);
        //        foreach (var report in reports)
        //        {
        //            report.Reviewed = true;
        //            report.ReviewedBy = editor;
        //        }
        //    }

        //    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<int> GetForumReactionCountByUserAsync(User user)
        //{
        //    return
        //        await
        //            this.databaseContext.SquareForumSubjectReactions.CountAsync(
        //                s => s.Creator.Id == user.Id).ConfigureAwait(false);
        //}

        public async Task<List<User>> SearchNeighborsNotAdminInSquareAsync(
            Square square,
            DbGeography position,
            int radius,
            string q)
        {
            return await (from users in this.databaseContext.Users
                //join zipcodes in this.databaseContext.SquareZipCodes.Where(s => s.Square.SquareId == square.SquareId) on users.PostalCode equals zipcodes.ZipCode
                where users.Position.Distance(position) <= radius
                      && this.databaseContext.SquareAdmins.Where(a => a.Square.SquareId == square.SquareId).All(a => a.User.Id != users.Id)
                          select users).ToListAsync().ConfigureAwait(false);
        }

        public async Task AddUserAsAdminAsync(Square square, User user, User invitedBy, string portalUrl)
        {
            if ((await this.IsUserSquareAdminAsync(square.SquareId, user).ConfigureAwait(false)))
            {
                return;
            }

            var squareInvitation = this.databaseContext.SquareAdmins.Create();
            squareInvitation.User = user;
            squareInvitation.Square = square;
            squareInvitation.CreatedBy = invitedBy;
            squareInvitation.CreatedDate = DateTime.Now;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            //await this.SendInvitationEmailAsync(circleInvitation, portalUrl).ConfigureAwait(false);
            //await this.SendNotificationInviteUserAsync(user, circle).ConfigureAwait(false);
        }

        public async Task<ForumReaction> GetSquareForumSubjectReactionByIdAsync(int? reactionId)
        {
            return
                await
                this.databaseContext.ForumReactions.Include(r => r.MediaList.Select(m => m.Media)).FirstOrDefaultAsync(c => c.ForumReactionId == reactionId).ConfigureAwait(false);
        }

        //public async Task RemoveReactionAsync(ForumReaction reaction, User deletedBy)
        //{
        //    reaction.Deleted = true;
        //    reaction.DeletedBy = deletedBy;
        //    reaction.DeletedDate = DateTime.Now;

        //    var reports = await this.GetReactionReportsAsync(reaction.ForumReactionId).ConfigureAwait(false);
        //    foreach (var report in reports)
        //    {
        //        report.Reviewed = true;
        //        report.ReviewedBy = deletedBy;
        //    }

        //    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task ReportReactionAsync(ForumReaction reaction, User reportedBy)
        //{
        //    var report = this.databaseContext.ForumReactionReports.Create();
        //    report.Reaction = reaction;
        //    report.Reporter = reportedBy;
        //    report.ReportDate = DateTime.Now;
        //    this.databaseContext.ForumReactionReports.Add(report);
        //    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

        //    //var reactionUrl = this.linkGenerator.GenerateMarketplaceReactionLink(
        //    //        marketplaceItem.MarketplaceItemId,
        //    //        reaction.ReactionId);

        //    //var model = new EmailTemplates.Models.MarketplaceReactionModel
        //    //{
        //    //    Subject =
        //    //                        string.Format(
        //    //                            Subject.MarketplaceReaction,
        //    //                            marketplaceItem.Title),
        //    //    PortalUrl = portalUrl,
        //    //    MarketplaceItemTitle =
        //    //                        marketplaceItem.Title,
        //    //    ReceiverName = emailToUser.Name,
        //    //    SenderName = creator.Name,
        //    //    ReactionUrl = reactionUrl
        //    //};

        //    //await this.mailerService.SendAsync(model, emailToUser).ConfigureAwait(false);

        //    //var message = string.Format(
        //    //    Common.InterfaceText.Notification.MarketplaceReaction,
        //    //    creator.Name,
        //    //    marketplaceItem.Title);
        //    //await
        //    //    this.notificationService.CreateNotificationForUserAsync(emailToUser, message, reactionUrl)
        //    //        .ConfigureAwait(false);

        //}

        //public async Task<ForumReactionReport> GetForumReactionReportByIdAsync(int? reportId)
        //{
        //    return await this.databaseContext.ForumReactionReports.FirstOrDefaultAsync(r => r.ForumReactionReportId == reportId).ConfigureAwait(false);
        //}

        //public async Task IgnoreReportedReactionAsync(ForumReactionReport report, User ignoredBy)
        //{
        //    report.Reviewed = true;
        //    report.ReviewedBy = ignoredBy;

        //    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        //}

        public async Task<bool> IsForumReactionReportedAsync(int? reactionId, User user)
        {
            return await this.databaseContext.ForumReactionReports.AnyAsync(r => r.Reaction.ForumReactionId == reactionId && r.Reporter.Id == user.Id && !r.Reviewed).ConfigureAwait(false);
        }

        public async Task ChangeForumSettingsAsync(Square square, string title, string text)
        {
            square.ForumTitle = title;
            square.ForumText = text;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeSquareGeneralAsync(Square square, string name, string infoTitle, string infoText, int coverColor, ImageDto profileImage, ImageDto coverImage)
        {
            square.Name = name;
            square.InfoTitle = infoTitle;
            square.InfoText = infoText;
            square.CoverColor = coverColor;

            if (profileImage != null)
            {
                if (square.ProfileImage != null)
                {
                    this.databaseContext.Media.Remove(square.ProfileImage);
                }

                square.ProfileImage = this.mediaService.CreateMedia(profileImage);
            }

            if (coverImage != null)
            {
                if (square.CoverImage != null)
                {
                    this.databaseContext.Media.Remove(square.CoverImage);
                }

                square.CoverImage = this.mediaService.CreateMedia(coverImage);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
