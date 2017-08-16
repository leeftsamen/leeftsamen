// <copyright file="ForumsController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.Data.Models;
using LeeftSamen.Portal.Services;
using LeeftSamen.Portal.Web.Extensions;
using LeeftSamen.Portal.Web.Helpers;
using LeeftSamen.Portal.Web.Models.Forums;
using LeeftSamen.Portal.Web.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace LeeftSamen.Portal.Web.Controllers
{
    public class ForumsController : BaseController
    {
        private readonly IForumService forumService;
        private readonly ICircleService circleService;
        private readonly IMediaService mediaService;
        private readonly ISquareService squareService;
        private readonly IUserService userService;

        public readonly int PageSize = 5;

        public ForumsController(ICurrentUserInformation currentUserInformation,
            IForumService forumService,
            ICircleService circleService,
            IMediaService mediaService,
            ISquareService squareService,
            IUserService userService
            ) : base(currentUserInformation)
        {
            this.forumService = forumService;
            this.circleService = circleService;
            this.mediaService = mediaService;
            this.squareService = squareService;
            this.userService = userService;
        }

        private async Task<bool> ForumExists(string type, int? typeId)
        {
            ModelType typeEnum;
            if (!Enum.TryParse<ModelType>(type, true, out typeEnum))
            {
                return false;
            }

            switch (typeEnum)
            {
                case ModelType.Circles:
                    var circle = await this.circleService.GetCircleByIdAsync(typeId, this.CurrentUser);
                    if (circle == null)
                    {
                        return false;
                    }

                    break;
                case ModelType.Squares:
                    var square = await this.squareService.GetSquareByIdAsync(typeId, this.CurrentUser);
                    if (square == null)
                    {
                        return false;
                    }

                    break;
                default:
                    return false;
            }

            return true;
        }

        private async Task<bool> UserIsForumAdmin(string type, int? typeId, User user)
        {
            ModelType typeEnum;
            if (!Enum.TryParse<ModelType>(type, true, out typeEnum))
            {
                return false;
            }

            switch (typeEnum)
            {
                case ModelType.Circles:
                    var circle = await this.circleService.GetCircleByIdAsync(typeId, user);
                    if (circle == null)
                    {
                        return this.circleService.IsUserAdminOfCircle(circle, user);
                    }

                    break;
                case ModelType.Squares:
                    return await this.squareService.IsUserSquareAdminAsync(typeId, user);
            }

            return false;
        }

        private async Task FillForumText(string type, int? typeId, IndexViewModel model)
        {
            ModelType typeEnum;
            if (!Enum.TryParse<ModelType>(type, true, out typeEnum))
            {
                return;
            }

            switch (typeEnum)
            {
                case ModelType.Squares:
                    var square = await this.squareService.GetSquareByIdAsync(typeId, this.CurrentUser);
                    if (square != null)
                    {
                        model.Title = square.ForumTitle;
                        model.Text = square.ForumText;
                    }

                    break;
            }
        }

        private Media PostedFileToMedia(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.IsImage())
            {
                return this.mediaService.CreateMedia(ImageHelper.GetResizedImage(file, 600, 600));
            }

            return this.mediaService.CreateMedia(file);
        }

        [HttpGet]
        public async Task<ActionResult> CreateReaction(int? id, string type, int? typeId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var subject = await this.forumService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return this.HttpNotFound();
            }

            var model = new CreateReactionViewModel();
            model.Type = type;
            model.TypeId = typeId.Value;
            model.SubjectId = subject.ForumSubjectId;
            return this.View(string.Format("CreateReaction{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateReaction(CreateReactionPostModel postedModel, string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var subject = await this.forumService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                var files = new List<Media>();
                foreach (string filename in this.Request.Files)
                {
                    var file = this.Request.Files[filename];
                    if (file != null && file.ContentLength > 0)
                    {
                        files.Add(this.PostedFileToMedia(file));
                    }
                }

                await
                    this.forumService.AddReactionAsync(this.CurrentUser, subject, postedModel.Title, postedModel.Text, files, false);

                return this.RedirectToAction("Subject", new { type, typeId, id = id });
            }

            var model = CreateReactionViewModel.FromPostModel(postedModel);
            model.Type = type;
            model.TypeId = typeId.Value;
            model.SubjectId = subject.ForumSubjectId;
            return this.View(string.Format("CreateReaction{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateSubject(string type, int? typeId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var model = new CreateSubjectViewModel();
            model.Type = type;
            model.TypeId = typeId.Value;
            return this.View(string.Format("CreateSubject{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSubject(CreateSubjectPostModel postedModel, string type, int? typeId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                var subject = await
                    this.forumService.AddSubjectAsync(this.CurrentUser, type, typeId.Value, postedModel.Title,
                        postedModel.Description);

                var files = new List<Media>();
                foreach (string filename in this.Request.Files)
                {
                    var file = this.Request.Files[filename];
                    if (file != null && file.ContentLength > 0)
                    {
                        files.Add(this.PostedFileToMedia(file));
                    }
                }

                await this.forumService.AddReactionAsync(this.CurrentUser, subject, postedModel.Title, postedModel.Text, files, true);

                return this.RedirectToAction("Index", new { type, typeId });
            }

            var model = CreateSubjectViewModel.FromPostModel(postedModel);
            return this.View(string.Format("CreateSubject{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> EditReaction(string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var reaction = await this.forumService.GetReactionByIdAsync(id);
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            int subjectId;
            if (reaction.Subject != null)
            {
                subjectId = reaction.Subject.ForumSubjectId;
            }
            else
            {
                subjectId = reaction.ParentReaction.Subject.ForumSubjectId;
            }

            var model = new EditReactionModel();
            model.Type = type;
            model.TypeId = typeId.Value;
            model.SubjectId = subjectId;
            model.ReactionId = reaction.ForumReactionId;
            model.Text = reaction.Message;
            model.Files = reaction.MediaList.Select(m => m.Media).ToList();
            model.AllowFiles = reaction.ParentReaction == null;
            return this.View(string.Format("EditReaction{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditReaction(EditReactionModel postedModel, string type, int? typeId, int? id)
        {
            if (this.ModelState.IsValid)
            {
                if (!await this.ForumExists(type, typeId))
                {
                    return this.HttpNotFound();
                }

                var reaction = await this.forumService.GetReactionByIdAsync(id);
                if (reaction == null)
                {
                    return this.HttpNotFound();
                }

                var oldFileIds = new List<int>();
                for (int i = 1; i <= reaction.MediaList.Count; i++)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(this.Request["file" + i]))
                        {
                            oldFileIds.Add(int.Parse(this.Request["file" + i]));
                        }
                    }
                    catch { }
                }

                var files = new List<Media>();
                foreach (string filename in this.Request.Files)
                {
                    var file = this.Request.Files[filename];
                    if (file != null && file.ContentLength > 0)
                    {
                        files.Add(this.PostedFileToMedia(file));
                    }
                }

                await
                    this.forumService.EditReactionAsync(reaction, postedModel.Text, files, oldFileIds, this.CurrentUser);

                int subjectId;
                if (reaction.Subject != null)
                {
                    subjectId = reaction.Subject.ForumSubjectId;
                }
                else
                {
                    subjectId = reaction.ParentReaction.Subject.ForumSubjectId;
                }

                return this.RedirectToRoute("Forum", new { controller = "Forums", action = "Subject", type, typeId, id = subjectId });
            }

            return this.View(string.Format("EditReaction{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postedModel);
        }

        [HttpGet]
        public async Task<ActionResult> EditSubject(string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var subject = await this.forumService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return this.HttpNotFound();
            }

            var model = new EditSubjectModel();
            model.Type = type;
            model.TypeId = typeId.Value;
            model.SubjectId = subject.ForumSubjectId;
            model.Title = subject.Title;
            model.Description = subject.Description;
            return this.View(string.Format("EditSubject{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubject(EditSubjectModel postedModel, string type, int? typeId, int? id)
        {
            if (this.ModelState.IsValid)
            {
                if (!await this.ForumExists(type, typeId))
                {
                    return this.HttpNotFound();
                }

                var subject = await this.forumService.GetSubjectByIdAsync(id);
                if (subject == null)
                {
                    return this.HttpNotFound();
                }

                await
                    this.forumService.EditSubjectAsync(subject, postedModel.Title, postedModel.Description);

                return this.RedirectToRoute("ForumNoId", new { controller = "Forums", action = "Index", type, typeId});
            }

            return this.View(string.Format("EditSubject{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postedModel);
        }

        [HttpGet]
        public async Task<ActionResult> IgnoreReportedReaction(string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var isAdmin = await this.UserIsForumAdmin(type, typeId, this.CurrentUser);
            if (!isAdmin)
            {
                return this.HttpForbidden();
            }

            var report = await this.forumService.GetReportedReactionAsync(id);
            if (report == null)
            {
                return this.HttpNotFound();
            }

            await this.forumService.IgnoreReportedReactionAsync(report, this.CurrentUser);

            this.NotifyUserSuccess(Alert.RemoveReactionReportSuccess);

            return this.RedirectToRoute("ForumNoId", new { controller = "Forums", action = "ReportedReactions", type, typeId });
        }

        public async Task<ActionResult> Index(string type, int? typeId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var subjects = new List<ForumSubject>();
            subjects = await this.forumService.GetSubjectsAsync(type, typeId.Value);
            var model = new IndexViewModel();
            model.IsCurrentUserAdmin = await this.UserIsForumAdmin(type, typeId, this.CurrentUser);
            model.Type = type;
            model.TypeId = typeId;
            await this.FillForumText(type, typeId, model);
            model.Subjects = subjects.Select(s =>
                new IndexViewModel.ForumSubject
                {
                    SubjectId = s.ForumSubjectId,
                    Title = s.Title,
                    Description = s.Description,
                }).ToList();
            foreach (var subject in model.Subjects)
            {
                subject.ReactionCount = await this.forumService.GetReactionCountWithChildsBySubjectIdAsync(subject.SubjectId);
                var lastReaction = await this.forumService.GetLastReactionBySubjectIdAsync(subject.SubjectId);
                if (lastReaction != null)
                {
                    subject.LastMessageName = lastReaction.Creator.Name;
                    subject.LastMessageDate = lastReaction.CreationDate;
                }
            }

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> ReactionImage(string type, int? typeId, int? id, int? reactionId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var reaction = await this.forumService.GetReactionByIdAsync(reactionId);
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            Media media = reaction.MediaList.Select(m => m.Media).FirstOrDefault(m => m.MediaId == id);
            if (media == null)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(media.Data);
            image.Resize(483, 483);

            // We make sure the height is never larger than 2 times the width
            int cropMargin = image.Height - (image.Width * 2);
            if (cropMargin > 1)
            {
                image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);
            }

            image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> ReactionImageLarge(string type, int? typeId, int? id, int? reactionId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var reaction = await this.forumService.GetReactionByIdAsync(reactionId);
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            Media media = reaction.MediaList.Select(m => m.Media).FirstOrDefault(m => m.MediaId == id);
            if (media == null)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(media.Data);

            // We make sure the height is never larger than 2 times the width
            int cropMargin = image.Height - (image.Width * 2);
            if (cropMargin > 1)
            {
                image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);
            }

            image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReactToReaction(SubjectViewModel postedModel, string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var reaction = await this.forumService.GetReactionByIdAsync(id);
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            await this.forumService.AddChildReactionAsync(this.CurrentUser, reaction, postedModel.ReactionText);
            int subjectId;
            if (reaction.Subject != null)
            {
                subjectId = reaction.Subject.ForumSubjectId;
            }
            else
            {
                subjectId = reaction.ParentReaction.Subject.ForumSubjectId;
            }

            return this.RedirectToRoute("Forum", new { controller = "Forums", action = "Subject", type, typeId, id = subjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveSubject(string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var subject = await this.forumService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return this.HttpNotFound();
            }

            var isAdmin = await this.UserIsForumAdmin(type, typeId, this.CurrentUser);
            if (!isAdmin)
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                await this.forumService.RemoveSubject(subject);

                this.NotifyUserSuccess(Alert.RemoveSubjectSuccess);
            }

            return this.RedirectToRoute("ForumNoId", new { controller = "Forums", action = "Index", type, typeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveReaction(string type, int? typeId, int? id, bool fromReported = false)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var reaction = await this.forumService.GetReactionByIdAsync(id);
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            var isAdmin = await this.UserIsForumAdmin(type, typeId, this.CurrentUser);
            if (!isAdmin)
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                await this.forumService.RemoveReactionAsync(reaction, this.CurrentUser);

                this.NotifyUserSuccess(Alert.RemoveReactionSuccess);
            }

            if (fromReported)
            {
                return this.RedirectToAction("ReportedReactions", new { id = id });
            }

            int subjectId;
            if (reaction.Subject != null)
            {
                subjectId = reaction.Subject.ForumSubjectId;
            }
            else
            {
                subjectId = reaction.ParentReaction.Subject.ForumSubjectId;
            }

            return this.RedirectToRoute("Forum", new { controller = "Forums", action = "Subject", type, typeId, id = subjectId });
        }

        [HttpGet]
        public async Task<ActionResult> ReportedReactions(string type, int? typeId)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var isAdmin = await this.UserIsForumAdmin(type, typeId, this.CurrentUser);
            if (!isAdmin)
            {
                return this.HttpForbidden();
            }

            var reported = await this.forumService.GetActiveReactionReportsAsync(type, typeId.Value);

            var model = new ReportedReactionsModel();
            model.Type = type;
            model.TypeId = typeId.Value;
            model.Reactions = reported.Select(r => new ReportedReactionsModel.ReportedReaction()
            {
                ReactionId = r.Reaction.ForumReactionId,
                ReportId = r.ForumReactionReportId,
                ReporterName = r.Reporter.Name,
                Title = r.Reaction.Title,
                Reaction = r.Reaction.Message,
                ReportedDate = r.ReportDate,
                ReactionByName = r.Reaction.Creator.Name,
                ReactionById = r.Reaction.Creator.Id,
                MediaList = r.Reaction.MediaList.Select(m => m.Media).ToList()
            }).ToList();
            return this.View(string.Format("ReportedReactions{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReportReaction(string type, int? typeId, int? id)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var reaction = await this.forumService.GetReactionByIdAsync(id);
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                var isReported = await this.forumService.IsReactionReportedAsync(reaction.ForumReactionId, this.CurrentUser);
                if (!isReported)
                {
                    await this.forumService.ReportReactionAsync(reaction, this.CurrentUser);
                }

                this.NotifyUserSuccess(Alert.ReportReactionSuccess);
            }

            int subjectId;
            if (reaction.Subject != null)
            {
                subjectId = reaction.Subject.ForumSubjectId;
            }
            else
            {
                subjectId = reaction.ParentReaction.Subject.ForumSubjectId;
            }

            return this.RedirectToRoute("Forum", new { controller = "Forums", action = "Subject", type, typeId, id = subjectId });
        }

        [HttpGet]
        public async Task<ActionResult> Subject(string type, int? typeId, int? id, int page = 1)
        {
            if (!await this.ForumExists(type, typeId))
            {
                return this.HttpNotFound();
            }

            var model = new SubjectViewModel();
            model.IsCurrentUserAdmin = await this.UserIsForumAdmin(type, typeId, this.CurrentUser);

            var subject = await this.forumService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return this.HttpNotFound();
            }

            var reactions = await this.forumService.GetReactionsBySubjectAsync(subject, this.PageSize, page);
            model.Type = type;
            model.TypeId = typeId.Value;
            model.SubjectId = subject.ForumSubjectId;
            model.SubjectName = subject.Title;
            model.SubjectText = subject.Description;
            model.SubjectDate = subject.CreationDate;
            model.Page = page;
            model.CurrentUserId = this.CurrentUser.Id;
            model.ProfileImageId = this.CurrentUser.ProfileImageId;
            var totalCount = await this.forumService.GetReactionCountBySubjectIdAsync(subject.ForumSubjectId);
            model.PageCount = Math.Max(totalCount / this.PageSize + (totalCount % this.PageSize > 0 ? 1 : 0), 1);
            var subjects = await this.forumService.GetSubjectsAsync(type, typeId.Value);
            model.Subjects = subjects.Where(s => s.ForumSubjectId != subject.ForumSubjectId)
            .Select(s => new SubjectViewModel.ForumSubject
            {
                SubjectId = s.ForumSubjectId,
                Title = s.Title,
                CreatedDate = s.CreationDate
            }).ToList();
            foreach (var childSubject in model.Subjects)
            {
                var lastReaction = await this.forumService.GetLastReactionBySubjectIdAsync(childSubject.SubjectId);
                if (lastReaction != null)
                {
                    childSubject.LastReactionDate = lastReaction.CreationDate;
                }
            }

            model.Subjects = model.Subjects.OrderByDescending(s => s.LastReactionDate ?? s.CreatedDate).Take(10).ToList();
            model.Reactions = reactions.Select(r => new SubjectViewModel.ForumReaction
            {
                ReactionId = r.ForumReactionId,
                CreatorId = r.Creator.Id,
                ProfileImageId = r.Creator.ProfileImageId,
                CreatorName = r.Creator.Name,
                CreationDate = r.CreationDate,
                Title = r.Title,
                Text = r.Message,
                MediaList = r.MediaList.Select(m => m.Media).ToList(),
                Deleted = r.Deleted,
                LastEditDate = r.LastEditDate
            }).ToList();

            var creatorDistances = new Dictionary<string, int>();
            foreach (var reaction in model.Reactions)
            {
                if (!creatorDistances.ContainsKey(reaction.CreatorId))
                {
                    var creator = await this.userService.GetUserByIdAsync(reaction.CreatorId);
                    creatorDistances[reaction.CreatorId] = (int)Math.Round((double)(creator.Position.Distance(this.CurrentUser.Position)), 1);
                }

                reaction.Distance = creatorDistances[reaction.CreatorId];
                reaction.Reported = await this.forumService.IsReactionReportedAsync(reaction.ReactionId, this.CurrentUser);

                var childReactions = await this.forumService.GetReactionsByParentReactionIdAsync(reaction.ReactionId);
                reaction.ChildReactions = childReactions.Select(r => new SubjectViewModel.ForumReaction
                {
                    ReactionId = r.ForumReactionId,
                    CreatorId = r.Creator.Id,
                    ProfileImageId = r.Creator.ProfileImageId,
                    CreatorName = r.Creator.Name,
                    CreationDate = r.CreationDate,
                    Text = r.Message,
                    MediaList = r.MediaList.Select(m => m.Media).ToList(),
                    Deleted = r.Deleted,
                    LastEditDate = r.LastEditDate
                }).ToList();
                foreach (var childReaction in reaction.ChildReactions)
                {
                    if (!creatorDistances.ContainsKey(childReaction.CreatorId))
                    {
                        var childCreator = await this.userService.GetUserByIdAsync(childReaction.CreatorId);
                        creatorDistances[childReaction.CreatorId] = (int)Math.Round((double)(childCreator.Position.Distance(this.CurrentUser.Position)), 1);
                    }

                    childReaction.Distance = creatorDistances[childReaction.CreatorId];
                    childReaction.Reported = await this.forumService.IsReactionReportedAsync(childReaction.ReactionId, this.CurrentUser);
                }
            }

            model.Reactions = model.Reactions.OrderByDescending(r => Math.Max(r.CreationDate.Ticks, r.ChildReactions.Select(c => c.CreationDate).DefaultIfEmpty(DateTime.MinValue).Max().Ticks)).ToList();

            return this.View(string.Format("Subject{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }
    }
}