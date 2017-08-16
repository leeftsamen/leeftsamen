// <copyright file="ISquareService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Data.Entity.Spatial;
using System.Web;
using LeeftSamen.Portal.Services.DTO;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface ISquareService
    {
        int[] GetCoverColors();

        bool IsUserSquareAdmin(int? squareId, User user);

        Task<List<Square>> GetUserSquaresAsync(User user);

        Task<Square> GetSquareByIdAsync(int? squareId, User user);

        Task<List<SquareZipCode>> GetSquareZipCodesAsync(Square square);

        Task AddSquareZipCodeAsync(Square square, string zipcode);

        Task RemoveSquareZipCodeAsync(Square square, int zipcodeId);

        Task<bool> IsUserSquareAdminAsync(int? squareId, User user);

        Task<List<User>> GetSquareAdminsAsync(int? squareId);

        Task RemoveAdminAsync(int? squareId, string userId);

        Task<SquareFact> GetSquareFactByIdAsync(int? factId);

        Task<List<SquareFact>> GetSquareFactsAsync(Square square);

        Task AddSquareFactAsync(User user, Square square, string title, string introductiontext, string fullText, List<Media> files);

        Task EditSquareFactAsync(SquareFact squareFact, string title, string introductiontext, string fullText, List<Media> files, List<int> oldFiles);

        Task RemoveSquareFactAsync(SquareFact fact);

        //Task AddSquareSubjectAsync(User user, Square square, string title, string description);

        //Task<ForumSubject> GetSquareForumSubjectByIdAsync(int? subjectId);

        //Task<List<ForumSubject>> GetSquareForumSubjectsAsync(Square square);

        //Task<List<ForumReaction>> GetSquareForumSubjectReactionsBySubjectAsync(ForumSubject subject, int pageSize, int page);

        //Task<ForumReaction> GetSquareForumSubjectReactionByIdAsync(int? reactionId);

        //Task RemoveReactionAsync(ForumReaction reaction, User deletedBy);

        //Task ReportReactionAsync(ForumReaction reaction, User reportedBy);

        //Task IgnoreReportedReactionAsync(ForumReactionReport report, User ignoredBy);

        //Task<int> GetForumReactionCountBySubjectAsync(int subjectId);

        //Task<ForumReaction> GetForumLastReactionBySubjectAsync(int subjectId);

        //Task<List<ForumReactionReport>> GetReactionReportsAsync(int reactionId);

        //Task<ForumReactionReport> GetForumReactionReportByIdAsync(int? reportId);

        //Task<List<ForumReactionReport>> GetActiveReactionReportsBySquareIdAsync(int squareId);

        //Task AddSquareForumReactionAsync(User user, ForumSubject subject, string text, List<Media> files);

        //Task EditSquareForumReactionAsync(ForumReaction reaction, string text, List<Media> files, List<int> oldFiles, User editor);

        //Task<int> GetForumReactionCountByUserAsync(User user);

        Task<List<User>> SearchNeighborsNotAdminInSquareAsync(Square square, DbGeography position, int radius, string q);

        Task AddUserAsAdminAsync(Square square, User user, User invitedBy, string portalUrl);

        Task<bool> IsForumReactionReportedAsync(int? reactionId, User user);

        Task ChangeSquareGeneralAsync(
            Square square,
            string name,
            string infoTitle,
            string infoText,
            int coverColor,
            ImageDto profileImage,
            ImageDto coverImage);

        Task ChangeForumSettingsAsync(Square square, string title, string text);
    }
}
