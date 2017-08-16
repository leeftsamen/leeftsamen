// <copyright file="SquaresController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using AutoMapper;
using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data.Models;
using LeeftSamen.Portal.Services;
using LeeftSamen.Portal.Web.Extensions;
using LeeftSamen.Portal.Web.Helpers;
using LeeftSamen.Portal.Web.Models;
using LeeftSamen.Portal.Web.Models.Squares;
using LeeftSamen.Portal.Web.Utils;
using System.IO;
using LeeftSamen.Portal.Data.Enums;
using System.Configuration;

namespace LeeftSamen.Portal.Web.Controllers
{
    public class SquaresController : BaseController
    {
        private readonly ISquareService squareService;

        private readonly IHelpIconService helpIconService;

        private readonly IUserService userService;

        private readonly IMediaService mediaService;

        private const int MaxPhotoImageSize = 600;

        private const int MaxCoverImageSize = 1000;

        private const int MaxProfileImageSize = 100;

        public readonly int PageSize = 5;

        public SquaresController(
            ICurrentUserInformation currentUserInformation,
            IUserService userService,
            ISquareService squareService,
            IHelpIconService helpIconService,
            IMediaService mediaService)
            : base(currentUserInformation)
        {
            this.userService = userService;
            this.squareService = squareService;
            this.helpIconService = helpIconService;
            this.mediaService = mediaService;
        }

        [ChildActionOnly]
        public ActionResult MenuSquares()
        {
            var squares = this.squareService.GetUserSquaresAsync(this.CurrentUser).Result;
            var model = new MenuSquaresModel()
            {
                SquaresCount = squares.Count
            };
            if (squares.Count > 0)
            {
                model.FirstSquareId = squares.First().SquareId;
            }

            return this.PartialView("_MenuSquares", model);
        }

        [ChildActionOnly]
        public ActionResult DetailHeader(int? id)
        {
            // No async/await here, because it's not supported by ASP.NET MVC in childactions.
            var square = this.squareService.GetSquareByIdAsync(id, this.CurrentUser).Result;
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var isUserAdmin = this.squareService.IsUserSquareAdmin(square.SquareId, this.CurrentUser);

            var menuItems = new List<MenuItemModel>();
            menuItems.Add(
                new MenuItemModel(
                    "DefaultDetail",
                    new RouteValueDictionary(new { id, controller = "Squares", action = "Detail" }),
                    Button.Info));
            menuItems.Add(
                new MenuItemModel(
                    "DefaultDetail",
                    new RouteValueDictionary(new { id, controller = "Squares", action = "Facts" }),
                    Button.Facts));
            menuItems.Add(
                new MenuItemModel(
                    "ForumNoId",
                    new RouteValueDictionary(new { type = ModelType.Squares.ToString(), typeId = id, controller = "Forums", action = "Index" }),
                    Button.Forum));
            menuItems.Add(
                new MenuItemModel(
                    "DefaultDetail",
                    new RouteValueDictionary(new { id, controller = "Squares", action = "Members" }),
                    Button.Admins));

            if (isUserAdmin)
            {
                menuItems.Add(
                    new MenuItemModel(
                        "ForumNoId",
                    new RouteValueDictionary(new { type = ModelType.Squares.ToString(), typeId = id, controller = "Forums", action = "ReportedReactions" }),
                        Button.ReportedReactions));
            }

            foreach (var item in menuItems)
            {
                item.Selected = ViewUtils.IsActiveAction(
                    this.ControllerContext.ParentActionViewContext,
                    item.RouteValues["controller"].ToString(),
                    item.RouteValues["action"].ToString())
                        || item.SubMenuItems.Any(subItem => ViewUtils.IsActiveAction(
                            this.ControllerContext.ParentActionViewContext, subItem.RouteValues["controller"].ToString(), subItem.RouteValues["action"].ToString()));
            }

            var model = Mapper.Map<DetailHeaderViewModel>(square);
            model.MenuItems = menuItems;
            model.HelpIcons = this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser).Result;
            model.IsUserAdmin = isUserAdmin;

            if (square.ProfileImage != null)
            {
                model.ProfileImageId = square.ProfileImage.MediaId;
            }

            if (square.CoverImage != null)
            {
                model.CoverImageId = square.CoverImage.MediaId;
            }

            return this.PartialView(string.Format("_DetailHeader{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var squares = await this.squareService.GetUserSquaresAsync(this.CurrentUser);
            foreach (var square in squares)
            {
                if (square.ProfileImage != null)
                {
                    square.ProfileImageId = square.ProfileImage.MediaId;
                }

                if (square.CoverImage != null)
                {
                    square.CoverImageId = square.CoverImage.MediaId;
                }
            }

            var model = new IndexViewModel()
            {
                Squares =
                                    Mapper.Map<List<IndexViewModel.SquareViewModel>>(squares),
                HelpIcons =
                                    await
                                    this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                        this.CurrentUser)
            };

            return this.View("Index", model);
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> CoverImage(int? id, int? mediaId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !mediaId.HasValue || square.CoverImage.MediaId != mediaId)
            {
                return this.HttpNotFound();
            }

            var profileImage = await this.mediaService.GetByIdAsync(mediaId.Value);
            if (profileImage == null)
            {
                return this.HttpNotFound();
            }

            // TODO: Refactor to generic resize method (maybe drop WebImage altogether)
            var image = new WebImage(profileImage.Data);
            image.Resize(MaxCoverImageSize, MaxCoverImageSize, true, true);
            image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            image.Write("image/jpeg");

            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> ProfileImage(int? id, int? mediaId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !mediaId.HasValue || square.ProfileImage.MediaId != mediaId)
            {
                return this.HttpNotFound();
            }

            var profileImage = await this.mediaService.GetByIdAsync(mediaId.Value);
            if (profileImage == null)
            {
                return this.HttpNotFound();
            }

            return this.ResizedProfileImage(profileImage.Data);
        }

        [HttpGet]
        public  async Task<ActionResult> Detail(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var model = new DetailViewModel()
            {
                SquareId = square.SquareId,
                Name = square.Name,
                Title = square.InfoTitle,
                Description = square.InfoText
            };
            return this.View(string.Format("Detail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateFact(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var model = new CreateFactViewModel();
            model.SquareId = square.SquareId;
            return this.View(string.Format("CreateFact{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFact(CreateFactPostModel postedModel, int? id)
        {
            if (this.ModelState.IsValid)
            {
                var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
                if (square == null)
                {
                    return this.HttpNotFound();
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
                    this.squareService.AddSquareFactAsync(this.CurrentUser, square, postedModel.Title, postedModel.IntroductionText, postedModel.FullText, files);

                return this.RedirectToAction("Facts", new {id = id});
            }

            var model = CreateFactViewModel.FromPostModel(postedModel);
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> EditFact(int? id, int? factId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var fact = await this.squareService.GetSquareFactByIdAsync(factId);
            if (fact == null)
            {
                return this.HttpNotFound();
            }

            var model = new EditFactModel();
            model.SquareId = square.SquareId;
            model.FactId = fact.SquareFactId;
            model.Title = fact.Title;
            model.IntroductionText = fact.IntroductionText;
            model.FullText = fact.FullText;
            model.Files = fact.MediaList.Select(m => m.Media).ToList();
            return this.View(string.Format("EditFact{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditFact(EditFactModel postedModel, int? id, int? factId)
        {
            if (this.ModelState.IsValid)
            {
                var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
                if (square == null)
                {
                    return this.HttpNotFound();
                }

                var fact = await this.squareService.GetSquareFactByIdAsync(factId);
                if (fact == null)
                {
                    return this.HttpNotFound();
                }

                var oldFileIds = new List<int>();
                for (int i = 1; i <= fact.MediaList.Count; i++)
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
                    this.squareService.EditSquareFactAsync(fact, postedModel.Title, postedModel.IntroductionText, postedModel.FullText, files, oldFileIds);

                return this.RedirectToAction("Facts", new { id = id });
            }

            return this.View(string.Format("EditFact{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postedModel);
        }

        [HttpGet]
        public async Task<ActionResult> Facts(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var model = new FactsViewModel();
            model.SquareId = square.SquareId;
            model.UserIsAdmin = await this.squareService.IsUserSquareAdminAsync(id, this.CurrentUser);
            var facts = await this.squareService.GetSquareFactsAsync(square);
            model.Facts =
                facts.Select(
                    f =>
                        new FactsViewModel.FactModel
                        {
                            Creator = f.Creator,
                            CreationDate = f.CreationDate,
                            FactId = f.SquareFactId,
                            IntroductionText = f.IntroductionText,
                            Title = f.Title,
                            OverviewImage = f.MediaList.FirstOrDefault() != null? f.MediaList.First().Media : null
                        }).ToList();
            return this.View(string.Format("Facts{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> FactDetail(int? id, int? factId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var fact = await this.squareService.GetSquareFactByIdAsync(factId);
            if (fact == null)
            {
                return this.HttpNotFound();
            }

            var userIsAdmin = await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser);
            var model = new FactDetailModel
            {
                UserIsAdmin = userIsAdmin,
                SquareId = fact.Square.SquareId,
                FactId = fact.SquareFactId,
                Title = fact.Title,
                Text = fact.FullText,
                CreationDate = fact.CreationDate,
                Distance = Math.Round((double)(fact.Creator.Position.Distance(this.CurrentUserPosition) / 1000), 1),
                Creator = fact.Creator,
                Files = fact.MediaList.Select(m => m.Media).ToList()
            };
            return this.View(string.Format("FactDetail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveFact(int? id, int? factId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var fact = await this.squareService.GetSquareFactByIdAsync(factId);
            if (fact == null)
            {
                return this.HttpNotFound();
            }

            await this.squareService.RemoveSquareFactAsync(fact);

            this.NotifyUserSuccess(Alert.RemoveFactSuccess);

            return this.RedirectToAction("Facts", new { id = id });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(int? factId, int? mediaId)
        {
            var fact = await this.squareService.GetSquareFactByIdAsync(factId);
            if (fact == null)
            {
                return this.HttpNotFound();
            }

            var file = fact.MediaList.Select(m => m.Media).FirstOrDefault(m => m.MediaId == mediaId);
            if (file == null)
            {
                return this.HttpNotFound();
            }

            return this.File(file.Data, file.MimeType, file.Name);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAllFiles(int? factId)
        {
            var fact = await this.squareService.GetSquareFactByIdAsync(factId);
            if (fact == null)
            {
                return this.HttpNotFound();
            }

            var memoryStream = new MemoryStream();
            //using (var memoryStream = new MemoryStream())
            //{
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in fact.MediaList)
                    {
                        var createdFile = archive.CreateEntry(file.Media.Name);
                        using (var streamWriter = new BinaryWriter(createdFile.Open()))
                        {
                            streamWriter.Write(file.Media.Data);
                        }
                    }
                }

            //}
            memoryStream.Position = 0;
            return this.File(memoryStream, "application/zip", String.Format("Feit{0}Bestanden_{1:yyyyMMddHHmmss}.zip", factId, DateTime.Now));
            //var streamResult = new FileStreamResult(memoryStream, "application/zip");
            //streamResult.FileDownloadName = String.Format("Feit{0}Bestanden_{1:yyyyMMddHHmmss}.zip", factId, DateTime.Now);
            //return streamResult;
        }

        //[HttpGet]
        //public async Task<ActionResult> Forum(int? id)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var subjects = await this.squareService.GetSquareForumSubjectsAsync(square);

        //    var model = new ForumViewModel();
        //    model.SquareId = square.SquareId;
        //    model.Title = square.ForumTitle;
        //    model.Text = square.ForumText;
        //    model.CurrentUserIsAdministrator =
        //        await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser);
        //    model.Subjects = subjects.Select(s =>
        //        new ForumViewModel.ForumSubject
        //        {
        //            SubjectId = s.SquareForumSubjectId,
        //            Title = s.Title,
        //            Description = s.Description,
        //        }).ToList();
        //    foreach (var subject in model.Subjects)
        //    {
        //        subject.ReactionCount = await this.squareService.GetForumReactionCountBySubjectAsync(subject.SubjectId);
        //        var lastReaction = await this.squareService.GetForumLastReactionBySubjectAsync(subject.SubjectId);
        //        if (lastReaction != null)
        //        {
        //            subject.LastMessageName = lastReaction.Creator.Name;
        //            subject.LastMessageDate = lastReaction.CreationDate;
        //        }
        //    }

        //    return View("Forum", model);
        //}

        [HttpGet]
        public async Task<ActionResult> CreateSubject(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var model = new CreateSubjectViewModel();
            model.SquareId = square.SquareId;
            return this.View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> CreateSubject(CreateSubjectPostModel postedModel, int? id)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //        if (square == null)
        //        {
        //            return this.HttpNotFound();
        //        }
        //        await
        //            this.squareService.AddSquareSubjectAsync(this.CurrentUser, square, postedModel.Title,
        //                postedModel.Description);

        //        return this.RedirectToAction("Forum", new { id = id });
        //    }
        //    var model = CreateSubjectViewModel.FromPostModel(postedModel);
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ForumSubject(int? id, int? subjectId, int page = 1)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var subject = await this.squareService.GetSquareForumSubjectByIdAsync(subjectId);
        //    if (subject == null)
        //    {
        //        return this.HttpNotFound();
        //    }
        //    var reactions = await this.squareService.GetSquareForumSubjectReactionsBySubjectAsync(subject, this.PageSize, page);
        //    var model = new ForumSubjectViewModel();
        //    model.SquareId = square.SquareId;
        //    model.SubjectId = subject.SquareForumSubjectId;
        //    model.SubjectName = subject.Title;
        //    model.Page = page;
        //    model.CurrentUserId = this.CurrentUser.Id;
        //    model.IsCurrentUserAdmin = await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser);
        //    var totalCount = await this.squareService.GetForumReactionCountBySubjectAsync(subject.SquareForumSubjectId);
        //    model.PageCount = Math.Max(totalCount/this.PageSize + (totalCount%this.PageSize > 0 ? 1 : 0), 1);
        //    model.Reactions = reactions.Select(r => new ForumSubjectViewModel.ForumReaction
        //    {
        //        ReactionId = r.ForumReactionId,
        //        CreatorId = r.Creator.Id,
        //        ProfileImageId = r.Creator.ProfileImageId,
        //        CreatorName = r.Creator.Name,
        //        CreationDate = r.CreationDate,
        //        Text = r.Message,
        //        MediaList = r.MediaList.Select(m => m.Media).ToList(),
        //        Deleted = r.Deleted,
        //        LastEditDate = r.LastEditDate
        //    }).ToList();
        //    foreach (var reaction in model.Reactions)
        //    {
        //        var creator = await this.userService.GetUserByIdAsync(reaction.CreatorId);
        //        reaction.CreatorReactionCount =
        //            await this.squareService.GetForumReactionCountByUserAsync(creator);
        //        reaction.Distance = (int) Math.Round((double) (creator.Position.Distance(this.CurrentUser.Position)), 1);
        //        reaction.Reported = await this.squareService.IsForumReactionReportedAsync(reaction.ReactionId, this.CurrentUser);
        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> CreateReaction(int? id, int? subjectId)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null)
        //    {
        //        return this.HttpNotFound();
        //    }
        //    var subject = await this.squareService.GetSquareForumSubjectByIdAsync(subjectId);
        //    if (subject == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var model = new CreateReactionViewModel();
        //    model.SquareId = square.SquareId;
        //    model.SubjectId = subject.SquareForumSubjectId;
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> CreateReaction(CreateReactionPostModel postedModel, int? id, int? subjectId)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //        if (square == null)
        //        {
        //            return this.HttpNotFound();
        //        }

        //        var subject = await this.squareService.GetSquareForumSubjectByIdAsync(subjectId);
        //        if (subject == null)
        //        {
        //            return this.HttpNotFound();
        //        }

        //        var files = new List<Media>();
        //        foreach (string filename in Request.Files)
        //        {
        //            var file = Request.Files[filename];
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                files.Add(PostedFileToMedia(file));
        //            }
        //        }

        //        await
        //            this.squareService.AddSquareForumReactionAsync(this.CurrentUser, subject, postedModel.Text, files);

        //        return this.RedirectToAction("ForumSubject", new { id = id, subjectId = subjectId });
        //    }
        //    var model = CreateReactionViewModel.FromPostModel(postedModel);
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> EditReaction(int? id, int? reactionId)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null)
        //    {
        //        return this.HttpNotFound();
        //    }
        //    var reaction = await this.squareService.GetSquareForumSubjectReactionByIdAsync(reactionId);
        //    if (reaction == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var model = new EditReactionModel();
        //    model.SquareId = square.SquareId;
        //    model.SubjectId = reaction.Subject.SquareForumSubjectId;
        //    model.ReactionId = reaction.ForumReactionId;
        //    model.Text = reaction.Message;
        //    model.Files = reaction.MediaList.Select(m => m.Media).ToList();
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditReaction(EditReactionModel postedModel, int? id, int? reactionId)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //        if (square == null)
        //        {
        //            return this.HttpNotFound();
        //        }

        //        var reaction = await this.squareService.GetSquareForumSubjectReactionByIdAsync(reactionId);
        //        if (reaction == null)
        //        {
        //            return this.HttpNotFound();
        //        }

        //        var oldFileIds = new List<int>();
        //        for (int i = 1; i <= reaction.MediaList.Count; i++)
        //        {
        //            try
        //            {
        //                if (!string.IsNullOrEmpty(Request["file" + i]))
        //                {
        //                    oldFileIds.Add(int.Parse(Request["file" + i]));
        //                }
        //            }
        //            catch { }
        //        }

        //        var files = new List<Media>();
        //        foreach (string filename in Request.Files)
        //        {
        //            var file = Request.Files[filename];
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                files.Add(PostedFileToMedia(file));
        //            }
        //        }

        //        await
        //            this.squareService.EditSquareForumReactionAsync(reaction, postedModel.Text, files, oldFileIds, this.CurrentUser);

        //        return this.RedirectToAction("ForumSubject", new { id = id, subjectId = reaction.Subject.SquareForumSubjectId });
        //    }
        //    return View(postedModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemoveReaction(int? id, int? reactionId, bool fromReported = false)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var reaction = await this.squareService.GetSquareForumSubjectReactionByIdAsync(reactionId);
        //    if (reaction == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var isAdmin = await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser);
        //    if (!isAdmin)
        //    {
        //        return this.HttpForbidden();
        //    }

        //    if (this.ModelState.IsValid)
        //    {
        //        if (reactionId.HasValue)
        //        {
        //            await this.squareService.RemoveReactionAsync(reaction, this.CurrentUser);
        //        }

        //        this.NotifyUserSuccess(Alert.RemoveReactionSuccess);
        //    }
        //    if (fromReported)
        //    {
        //        return this.RedirectToAction("ReportedReactions", new { id = id });
        //    }
        //    return this.RedirectToAction("ForumSubject", new { id = id, subjectId = reaction.Subject.SquareForumSubjectId });
        //}

        //[HttpGet]
        //public async Task<ActionResult> IgnoreReportedReaction(int? id, int? reportId)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null || !reportId.HasValue)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var isAdmin = await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser);
        //    if (!isAdmin)
        //    {
        //        return this.HttpForbidden();
        //    }

        //    var report = await this.squareService.GetForumReactionReportByIdAsync(reportId);
        //    if (report == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    await this.squareService.IgnoreReportedReactionAsync(report, this.CurrentUser);

        //    this.NotifyUserSuccess(Alert.RemoveReactionReportSuccess);

        //    return this.RedirectToAction("ReportedReactions", new { id = id });

        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ReportReaction(int? id, int? reactionId)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var reaction = await this.squareService.GetSquareForumSubjectReactionByIdAsync(reactionId);
        //    if (reaction == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    if (this.ModelState.IsValid)
        //    {
        //        if (reactionId.HasValue)
        //        {
        //            var isReported = await this.squareService.IsForumReactionReportedAsync(reaction.ForumReactionId, this.CurrentUser);
        //            if (!isReported)
        //                await this.squareService.ReportReactionAsync(reaction, this.CurrentUser);
        //        }

        //        this.NotifyUserSuccess(Alert.ReportReactionSuccess);
        //    }

        //    return this.RedirectToAction("ForumSubject", new { id = id, subjectId = reaction.Subject.SquareForumSubjectId });
        //}

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> FactImage(int? factId, int? mediaId)
        {
            var fact = await this.squareService.GetSquareFactByIdAsync(factId);
            if (fact == null)
            {
                return this.HttpNotFound();
            }

            Media media = fact.MediaList.Select(m => m.Media).FirstOrDefault(m => m.MediaId == mediaId);
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

        //[HttpGet]
        //[AllowAnonymous]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        //public async Task<ActionResult> ReactionImage(int? reactionId, int? mediaId)
        //{
        //    var reaction = await this.squareService.GetSquareForumSubjectReactionByIdAsync(reactionId);
        //    if (reaction == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    Media media = reaction.MediaList.Select(m => m.Media).FirstOrDefault(m => m.MediaId == mediaId);
        //    if (media == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var image = new WebImage(media.Data);
        //    image.Resize(483, 483);

        //    // We make sure the height is never larger than 2 times the width
        //    int cropMargin = image.Height - (image.Width * 2);
        //    if (cropMargin > 1)
        //        image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);

        //    image.Crop(1, 1, 1, 1); // border bugfix in WebImage
        //    return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        //public async Task<ActionResult> ReactionImageLarge(int? reactionId, int? mediaId)
        //{
        //    var reaction = await this.squareService.GetSquareForumSubjectReactionByIdAsync(reactionId);
        //    if (reaction == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    Media media = reaction.MediaList.Select(m => m.Media).FirstOrDefault(m => m.MediaId == mediaId);
        //    if (media == null)
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var image = new WebImage(media.Data);

        //    // We make sure the height is never larger than 2 times the width
        //    int cropMargin = image.Height - (image.Width * 2);
        //    if (cropMargin > 1)
        //        image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);

        //    image.Crop(1, 1, 1, 1); // border bugfix in WebImage
        //    return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        //}

        [HttpGet]
        public async Task<ActionResult> Members(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var model = new MembersViewModel
            {
                SquareId = square.SquareId,
                SquareName = square.Name,
                CurrentUserIsAdministrator = await
                                    this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser),
                HelpIcons =
                                    await
                                    this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                        this.CurrentUser)
            };

            var members = await this.squareService.GetSquareAdminsAsync(square.SquareId);
            model.Admins.AddRange(Mapper.Map<List<MembersViewModel.MemberViewModel>>(members));

            return this.View(string.Format("Members{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Removemember(int? id, string memberId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var isAdmin = await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser);
            if (!isAdmin)
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(memberId))
                {
                    await this.squareService.RemoveAdminAsync(square.SquareId, memberId);
                }

                this.NotifyUserSuccess(Alert.RemoveAdminFromSquareSuccess);
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Squares", id, action = "Members" });
        }

        public async Task<ActionResult> SearchUsers(int? id, string q)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.HttpNotFound();
            }

            var results = await
                this.squareService.SearchNeighborsNotAdminInSquareAsync(
                    square,
                    this.CurrentUserPosition,
                    this.CurrentUserNeighborhoodRadius,
                    q);

            var users = Mapper.Map<List<SearchUserViewModel>>(results);
            foreach (var user in users.Where(u => u.HasProfileImage))
            {
                user.ProfileImageUrl = this.Url.RouteUrl(
                    "UserProfileImage",
                    new
                    {
                        controller = "Users",
                        action = "ProfileImage",
                        userId = user.UserId,
                        mediaId = user.ProfileImageId
                    });
            }

            return this.Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(int? id, string userId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            if (!await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser))
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            var user = await this.userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            await this.squareService.AddUserAsAdminAsync(square, user, this.CurrentUser, this.PortalUrl);

            return this.Json(new { state = "success", message = String.Format(Alert.UserIsInvitedForTheSquare, user.Name) });
        }

        [HttpGet]
        public async Task<ActionResult> Settings(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            var model = new SettingsModel()
            {
                SquareId = square.SquareId
            };
            return this.View(string.Format("Settings{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeGeneral(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            var model = new ChangeGeneralModel
            {
                Square = square,
                Name = square.Name,
                InfoTitle = square.InfoTitle,
                InfoText = square.InfoText,
                CoverColors = this.squareService.GetCoverColors(),
                CoverColor = square.CoverColor
            };
            return this.View(string.Format("ChangeGeneral{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),  model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeGeneral(int? id, ChangeGeneralModel model)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                var coverColors = this.squareService.GetCoverColors();
                var coverColor = coverColors[0];
                foreach (var t in coverColors.Where(t => model.CoverColor == t))
                {
                    coverColor = t;
                }

                var profileImage = ImageHelper.GetResizedImage(
                    model.ProfileImage,
                    MaxProfileImageSize,
                    MaxProfileImageSize);
                if (profileImage == null && model.ProfileImage != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    model.Square = square;
                    model.CoverColors = this.squareService.GetCoverColors();
                    return this.View(model);
                }

                var coverImage = ImageHelper.GetResizedImage(model.CoverImage, MaxCoverImageSize, MaxCoverImageSize);
                if (coverImage == null && model.CoverImage != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    model.Square = square;
                    model.CoverColors = this.squareService.GetCoverColors();
                    return this.View(model);
                }

                await
                    this.squareService.ChangeSquareGeneralAsync(
                        square,
                        model.Name,
                        model.InfoTitle,
                        model.InfoText,
                        coverColor,
                        profileImage,
                        coverImage);

                return this.RedirectToAction("Settings", new { id = square.SquareId });
            }

            this.SetStatusCode(HttpStatusCode.BadRequest);
            model.Square = square;
            model.CoverColors = this.squareService.GetCoverColors();
            return this.View(string.Format("ChangeGeneral{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeForum(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            var model = new ChangeForumModel()
            {
                SquareId = square.SquareId,
                Title = square.ForumTitle,
                Text = square.ForumText
            };
            return this.View(string.Format("ChangeForum{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeForum(ChangeForumModel postedModel, int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                await this.squareService.ChangeForumSettingsAsync(square, postedModel.Title, postedModel.Text);

                this.NotifyUserSuccess(Alert.ChangeForumSuccess);
                return this.RedirectToAction("Settings", new { id = square.SquareId });
            }

            return this.View(postedModel);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeZipCodes(int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            var model = new ChangeZipCodesModel()
            {
                SquareId = square.SquareId,
                Zipcodes = (await this.squareService.GetSquareZipCodesAsync(square)).Select(z => new ChangeZipCodesModel.ZipCodeModel() { ZipcodeId = z.SquareZipCodeId, ZipCode = z.ZipCode }).ToList()
            };
            return this.View(string.Format("ChangeZipCodes{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeZipCodes(ChangeZipCodesModel postedModel, int? id)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                await this.squareService.AddSquareZipCodeAsync(square, postedModel.NewZipCode);
                this.NotifyUserSuccess(Alert.AddZipCodeSuccess);
                postedModel.NewZipCode = string.Empty;
                this.ModelState.Clear();
            }

            if (id.HasValue)
            {
                postedModel.SquareId = id.Value;
            }

            postedModel.Zipcodes = (await this.squareService.GetSquareZipCodesAsync(square)).Select(z => new ChangeZipCodesModel.ZipCodeModel() { ZipcodeId = z.SquareZipCodeId, ZipCode = z.ZipCode }).ToList();
            return this.View(string.Format("ChangeZipCodes{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postedModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveZipcode(int? id, int? zipcodeId)
        {
            var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
            if (square == null || !zipcodeId.HasValue || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
            {
                return this.HttpNotFound();
            }

            await this.squareService.RemoveSquareZipCodeAsync(square, zipcodeId.Value);
            this.NotifyUserSuccess(Alert.RemoveZipCodeSuccess);
            return this.RedirectToAction("ChangeZipCodes", new { id = square.SquareId });
        }

        //[HttpGet]
        //public async Task<ActionResult> ReportedReactions(int? id)
        //{
        //    var square = await this.squareService.GetSquareByIdAsync(id, this.CurrentUser);
        //    if (square == null || !(await this.squareService.IsUserSquareAdminAsync(square.SquareId, this.CurrentUser)))
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var reported = await this.squareService.GetActiveReactionReportsBySquareIdAsync(square.SquareId);

        //    var model = new ReportedReactionsModel();
        //    model.SquareId = square.SquareId;
        //    model.Reactions = reported.Select(r => new ReportedReactionsModel.ReportedReaction()
        //    {
        //        ReactionId = r.Reaction.ForumReactionId,
        //        ReportId = r.ForumReactionReportId,
        //        ReporterName = r.Reporter.Name,
        //        Reaction = r.Reaction.Message,
        //        ReportedDate = r.ReportDate,
        //        ReactionByName = r.Reaction.Creator.Name,
        //        ReactionById = r.Reaction.Creator.Id,
        //        MediaList = r.Reaction.MediaList.Select(m => m.Media).ToList()
        //    }).ToList();
        //    return View(model);
        //}

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
    }
}