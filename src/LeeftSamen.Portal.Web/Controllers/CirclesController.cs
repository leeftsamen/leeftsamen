// <copyright file="CirclesController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Helpers;
    using LeeftSamen.Portal.Web.Models;
    using LeeftSamen.Portal.Web.Models.Circles;
    using LeeftSamen.Portal.Web.Utils;
    using Microsoft.Owin.Security;
    using Services.DTO;
    using System.Web;
    using Extensions;
    using Data.Enums;
    public class CirclesController : BaseController
    {
        private const int MaxAttachmentImageSize = 300;

        private const int MaxPhotoImageSize = 600;

        private const int MaxCoverImageSize = 1000;

        private const int MaxProfileImageSize = 100;

        private readonly ICircleService circleService;

        private readonly IHelpIconService helpIconService;

        private readonly IMediaService mediaService;

        private readonly IUserService userService;

        private readonly IAuthenticationManager authenticationManager;

        private readonly ISharedService sharedService;

        private readonly INotificationService notificationService;

        private readonly ILinkGenerator linkGenerator;

        public CirclesController(
            ICurrentUserInformation currentUserInformation,
            IUserService userService,
            ICircleService circleService,
            IHelpIconService helpIconService,
            IMediaService mediaService,
            IAuthenticationManager authenticationManager,
            ISharedService sharedService,
            INotificationService notificationService,
            ILinkGenerator linkGenerator)
            : base(currentUserInformation)
        {
            this.userService = userService;
            this.circleService = circleService;
            this.helpIconService = helpIconService;
            this.mediaService = mediaService;
            this.authenticationManager = authenticationManager;
            this.sharedService = sharedService;
            this.notificationService = notificationService;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult> AcceptInvitation(int? id, string code)
        {
            var invitation = await this.circleService.GetInvitationByCircleIdAcceptTokenAsync(id, code);

            if (invitation == null)
            {
                this.NotifyUserDanger(Error.InvitationDoesntExist);
                return this.RedirectToAction("Index", "Home");
            }

            if (invitation.User == null)
            {
                invitation.User = await this.userService.FindByEmailAsync(this.CurrentUser.Email);
            }

            if (invitation.User == null)
            {
                this.NotifyUserDanger(Error.AccountDoesntExist);
                return this.RedirectToAction("Index", "Home");
            }

            if (invitation.Circle == null)
            {
                this.NotifyUserDanger(Error.CircleDoesntExist);
                return this.RedirectToAction("Index", "Home");
            }

            //if (this.CurrentUser.Id != invitation.User.Id)
            //{
            //    MvcApplication.DeleteSession();
            //    this.authenticationManager.SignOut();

            //    var inivationUrl = this.linkGenerator.GenerateLocalCircleAcceptInvitationLink(
            //    invitation.Circle.CircleId,
            //    invitation.AcceptToken);

            //    this.NotifyUserDanger(Error.InvitationWrongUser);

            //    return this.RedirectToAction("Login", "Account", new { returnUrl = inivationUrl });
            //}

            // Right user but to late
            if (DateTime.Now > invitation.ExpireDate && invitation.User.Id == this.CurrentUser.Id)
            {
                this.NotifyUserDanger(Error.InvitationExpired);
                return this.RedirectToAction("Index", "Home");
            }

            // Wrong user
            if (invitation.User.Id != this.CurrentUser.Id)
            {
                this.NotifyUserDanger(Error.InvitationWrongUserAccepted);
                return this.RedirectToAction("Index", "Home");
            }

            //User already member of circle
            if (this.circleService.IsUserMemberOfCircle(invitation.Circle, invitation.User))
            {
                this.NotifyUserDanger(Error.UserAlreadyMemberOfCircle);
                return this.RedirectToAction("Index", "Home");
            }

            var membership = await this.circleService.AcceptInvitationAsync(invitation, this.CurrentUser);
            this.NotifyUserSuccess(Alert.YouAreNowACircleMember, membership.Circle.Name);

            return this.RedirectToAction("Detail", "Circles", new { id = membership.Circle.CircleId });
        }

        [HttpGet]
        public async Task<ActionResult> AcceptJoinRequest(int? id, string code)
        {
            // TODO: Refactor this method (used as link in email) to the 'JoinRequest' overview page.
            var joinRequest = await this.circleService.GetJoinRequestByCircleIdAcceptTokenAsync(id, code);
            if (joinRequest == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            if (!this.circleService.IsUserAdminOfCircle(joinRequest.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var membership = await this.circleService.AcceptJoinRequestAsync(joinRequest);
            this.NotifyUserSuccess(Alert.JoinRequestAccepted, membership.Circle.Name);

            return this.RedirectToRoute(
                "DefaultDetail",
                new { controller = "Circles", action = "Members", id = membership.Circle.CircleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcceptJoinRequest(int? id, int? requestId)
        {
            var joinRequest = await this.circleService.GetJoinRequestByIdAsync(id, requestId);
            if (joinRequest == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(joinRequest.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.AcceptJoinRequestAsync(joinRequest);
            this.NotifyUserSuccess(Alert.JoinRequestAccepted);

            return this.RedirectToAction("JoinRequests", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhotos(int id, int? albumId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            //if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            //{
            //    return this.HttpForbidden();
            //}

            if (this.Request.Files.Count > 0)
            {
                List<ImageDto> images = new List<ImageDto>();
                for (int i = 0; i < this.Request.Files.Count; i++)
                {
                    var image = this.Request.Files[i];

                    var photo = ImageHelper.GetResizedImage(image, MaxPhotoImageSize, MaxPhotoImageSize);
                    if (photo == null && image != null)
                    {
                        this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                        this.SetStatusCode(HttpStatusCode.BadRequest);
                        return this.RedirectToAction("Photos", "Circles", new { id });
                    }

                    images.Add(photo);
                }

                if (images.Count > 0)
                {
                    foreach (var photo in images)
                    {
                        await this.circleService.CreateCirclePhotoAsync(photo, circle, this.CurrentUser, albumId);
                    }

                    this.NotifyUserSuccess(Alert.CirclePhotoUploaded);
                    if (images.Count > 1)
                    {
                        this.NotifyUserSuccess(string.Format(Alert.CirclePhotosUploaded, images.Count));
                    }
                    else
                    {
                        this.NotifyUserSuccess(Alert.CirclePhotoUploaded);
                    }
                }

                this.NotifyUserSuccess(Alert.CirclePhotoUploaded);
            }

            return this.RedirectToRoute(
                "DefaultDetail",
                new
                {
                    controller = "Circles",
                    action = "Photos",
                    id = circle.CircleId,
                    photoAlbumId = albumId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDocuments(int id, int? albumId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            //if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            //{
            //    return this.HttpForbidden();
            //}

            if (this.Request.Files.Count > 0)
            {
                List<Media> documents = new List<Media>();
                for (int i = 0; i < this.Request.Files.Count; i++)
                {
                    var file = this.Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var media = this.PostedFileToMedia(file);
                        if (string.IsNullOrEmpty(media.FontAwesomeClass()))
                        {
                            this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                            this.SetStatusCode(HttpStatusCode.BadRequest);
                            return this.RedirectToAction("Documents", "Circles", new { id });
                        }

                        documents.Add(media);
                    }
                }

                if (documents.Count > 0)
                {
                    foreach (var file in documents)
                    {
                        await this.circleService.CreateCircleDocumentAsync(file, circle, this.CurrentUser, albumId);
                    }

                    if (documents.Count > 1)
                    {
                        this.NotifyUserSuccess(string.Format(Alert.CircleDocsUploaded, documents.Count));
                    }
                    else
                    {
                        this.NotifyUserSuccess(Alert.CircleDocUploaded);
                    }
                }
            }

            return this.RedirectToRoute(
                "DefaultDetail",
                new
                {
                    controller = "Circles",
                    action = "Documents",
                    id = circle.CircleId,
                    photAlbumId = albumId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhotoAlbum(int? id, int? photoAlbumId, CirclePhotosViewModel model)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (!this.ModelState.IsValid)
            {
                // TODO: Add proper validation handling, meegeven aan Photos()
                this.NotifyUserDanger(Alert.CantCreatePhotoAlbum);
            }
            else
            {
                await this.circleService.CreateCirclePhotoAlbumAsync(circle, model.Title, this.CurrentUser);

                this.NotifyUserSuccess(string.Format(Alert.CirclePhotoAlbumCreated, model.Title));
            }

            return this.RedirectToRoute(
                "DefaultDetail",
                new { controller = "Circles", action = "Photos", circleId = circle.CircleId });
        }

        [HttpGet]
        public async Task<ActionResult> Agenda(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);

            var model = new CircleAgendaViewModel
                            {
                                CircleId = circle.CircleId,
                                CircleIsPrivate = circle.IsPrivate,
                                CircleName = circle.Name,
                                CircleActivities =
                                    await
                                    this.circleService.GetCircleActivitiesByIdAsync(circle.CircleId),
                                CurrentUserIsCircleAdministrator =
                                    this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser),
                            };

            return this.View("Agenda", model);
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> Attachment(int? id, int? reactionId, int? mediaId)
        {
            if (id.HasValue)
            {
                var attachment = await this.circleService.GetAttachmentByCircleMessageIdAsync(this.CurrentUser, id, mediaId);
                if (attachment == null)
                {
                    return this.HttpNotFound();
                }

                return this.ResizedAttachment(attachment.Data);
            }
            else if (reactionId.HasValue)
            {
                var attachment = await this.circleService.GetAttachmentByCircleMessageReactionIdAsync(this.CurrentUser, reactionId, mediaId);
                if (attachment == null)
                {
                    return this.HttpNotFound();
                }

                return this.ResizedAttachment(attachment.Data);
            }
            else
            {
                return this.HttpNotFound();
            }
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> AttachmentLarge(int? id, int? reactionId, int? mediaId)
        {
            if (id.HasValue)
            {
                var attachment = await this.circleService.GetAttachmentByCircleMessageIdAsync(this.CurrentUser, id, mediaId);
                if (attachment == null)
                {
                    return this.HttpNotFound();
                }

                return this.File(attachment.Data, "image/jpeg");
            }
            else if (reactionId.HasValue)
            {
                var attachment = await this.circleService.GetAttachmentByCircleMessageReactionIdAsync(this.CurrentUser, reactionId, mediaId);
                if (attachment == null)
                {
                    return this.HttpNotFound();
                }

                return this.File(attachment.Data, "image/jpeg");
            }
            else
            {
                return this.HttpNotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ChangeMemberProfile(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            var timeLineItems = new List<DetailViewModel.TimelineItem>();

            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var memberShip = await this.circleService.GetCircleMembershipByIdAsync(circle.CircleId, this.CurrentUser.Id);
            if (memberShip == null)
            {
                return this.HttpForbidden();
            }

            return this.View("ChangeMemberProfile",
                new ChangeMemberProfileModel {
                    CircleId = circle.CircleId,
                    ProfileText = memberShip.Profile
                }
            );
        }

        [HttpPost]
        public async Task<ActionResult> ChangeMemberProfile(int? id, ChangeMemberProfileModel model)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            var timeLineItems = new List<DetailViewModel.TimelineItem>();

            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var memberShip = await this.circleService.GetCircleMembershipByIdAsync(circle.CircleId, this.CurrentUser.Id);
            if (memberShip == null)
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                await this.circleService.EditMembershipProfile(memberShip, model.ProfileText);
                return this.RedirectToAction("Members", new { id = circle.CircleId });
            }

            return this.View("ChangeMemberProfile", model);
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> CoverImage(int? id, int? mediaId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (circle.IsPrivate && !this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpNotFound();
            }


            var profileImage = await this.circleService.GetCoverImageByCircleIdAsync(id, mediaId);
            if (profileImage == null)
            {
                return this.HttpNotFound();
            }

            // TODO: Refactor to generic resize method (maybe drop WebImage altogether)
            var image = new WebImage(profileImage.Data);
            image.Resize(1000, 1000, true, true);
            image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            image.Write("image/jpeg");

            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            var model = new CreatePostModel { CoverColors = this.circleService.GetCoverColors() };
            return this.View(string.Format("Create{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreatePostModel model)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            // set coverColors in the model in case of a invalid model
            model.CoverColors = this.circleService.GetCoverColors();
            if (this.ModelState.IsValid)
            {
                var coverColor = model.CoverColors[0];
                foreach (var t in model.CoverColors.Where(t => model.CoverColor == t))
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
                    return this.View(string.Format("Create{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
                }

                var coverImage = ImageHelper.GetResizedImage(model.CoverImage, MaxCoverImageSize, MaxCoverImageSize);
                if (coverImage == null && model.CoverImage != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    return this.View(string.Format("Create{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
                }

                var circle =
                    await
                    this.circleService.CreateCircleAsync(
                        model.Name,
                        model.Description,
                        model.IsPrivate,
                        this.CurrentUser,
                        coverColor,
                        profileImage,
                        coverImage);

                this.NotifyUserSuccess(Alert.CircleCreated);

                return this.RedirectToAction("Detail", new { id = circle.CircleId });
            }

            this.SetStatusCode(HttpStatusCode.BadRequest);
            return this.View(string.Format("Create{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateActivity(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            var model = new CreateActivityModel { CircleId = circle.CircleId };

            return this.View("CreateActivity", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCircleActivity(CreateActivityModel activityModel)
        {
            var circle = await this.circleService.GetCircleByIdAsync(activityModel.CircleId);

            var activity = await this.circleService.CreateCircleActivityAsync(
                title: activityModel.Title,
                description: activityModel.Description,
                startDateTime: activityModel.StartDateTime,
                endDateTime: activityModel.EndDateTime,
                location: activityModel.Location,
                allDay: activityModel.AllDay,
                creator: this.CurrentUser,
                circle: circle,
                allAges: activityModel.AllAges,
                ageFrom: activityModel.AgeFrom ?? 0 ,
                ageTo: activityModel.AgeTo ?? 0
            );

            this.NotifyUserSuccess(Alert.ActivityCreated);
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Agenda" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEmailMessage(CircleCreateEmailMessageModel model,
            List<string> checkedUsersIdList)
        {
            List<User> membersToSendTo = new List<User>();

            var circle = await this.circleService.GetCircleByIdAsync(model.CircleId);

            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (checkedUsersIdList == null)
            {
                this.NotifyUserDanger(Error.RecipientIsRequired);
                return this.RedirectToRoute(
                    "DefaultDetail",
                    new { controller = "Circles", action = "CreateEmailMessage", id = circle.CircleId });
            }
            else
            {
                if (checkedUsersIdList.Count > 0)
                {
                    foreach (var memberName in checkedUsersIdList)
                    {
                        var user = await this.userService.GetUserByIdAsync(memberName);
                        membersToSendTo.Add(user);
                    }
                }
                else
                {
                    this.NotifyUserDanger(Error.RecipientIsRequired);
                    return this.RedirectToRoute(
                        "DefaultDetail",
                        new { controller = "Circles", action = "CreateEmailMessage", id = circle.CircleId });
                }
            }

            if (this.ModelState.IsValid)
            {
                await
                    this.circleService.CreateEmailMessageAsync(
                        model.messageText,
                        model.subjectText,
                        this.CurrentUser,
                        membersToSendTo,
                        circle, this.PortalUrl);
                this.NotifyUserSuccess(Alert.MessageSendSuccess);
            }
            else
            {
                model.CircleName = circle.Name;
                model.CircleUsers = circle.Members;
                model.CurrentUser = this.CurrentUser;
                model.CheckedUsers = checkedUsersIdList;
                return this.View("CreateEmailMessage", model);
            }

            return this.RedirectToRoute(
                "DefaultDetail",
                new { controller = "Circles", action = "InboxReceived", id = circle.CircleId });
        }

        [HttpGet]
        public async Task<ActionResult> CreateEmailMessage(int? id, int? messageId, bool reactToAll = false)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var message = await this.circleService.GetEmailMessageAsync(messageId);
            if (messageId.HasValue && (message == null || !(message.Recipients.Any(r => r.Receiver.Id == this.CurrentUser.Id))))
            {
                return this.HttpNotFound();
            }

            var model = new CircleCreateEmailMessageModel
                            {
                                CircleId = circle.CircleId,
                                CircleName = circle.Name,
                                CircleUsers = circle.Members,
                                CurrentUser = this.CurrentUser
                            };

            if (message != null)
            {
                var subject = message.Subject;
                if (!subject.StartsWith(Label.MessageReplied))
                {
                    subject = Label.MessageReplied + subject;
                }

                var checkedUsers = new List<string>();
                if (reactToAll)
                {
                    checkedUsers = message.Recipients.Select(r => r.Receiver.Id).ToList();
                }

                checkedUsers.Add(message.Creator.Id);
                model.subjectText = subject;
                model.CheckedUsers = checkedUsers;
            }

            return this.View("CreateEmailMessage", model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateEmailGroup(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var model = new CreateEmailGroupModel
            {
                CircleId = circle.CircleId,
                CircleName = circle.Name,
                CircleUsers = circle.Members,
                CurrentUser = this.CurrentUser
            };

            return this.View(string.Format("CreateEmailGroup{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEmailGroup(CreateEmailGroupModel model,
            List<string> checkedUsersIdList)
        {
            List<User> membersToSendTo = new List<User>();

            var circle = await this.circleService.GetCircleByIdAsync(model.CircleId);

            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (checkedUsersIdList == null)
            {
                this.NotifyUserDanger(Error.RecipientIsRequired);
                return this.RedirectToRoute(
                    "DefaultDetail",
                    new { controller = "Circles", action = "CreateEmailGroup", id = circle.CircleId });
            }
            else
            {
                if (checkedUsersIdList.Count > 0)
                {
                    foreach (var memberName in checkedUsersIdList)
                    {
                        var user = await this.userService.GetUserByIdAsync(memberName);
                        membersToSendTo.Add(user);
                    }
                }
                else
                {
                    this.NotifyUserDanger(Error.RecipientIsRequired);
                    return this.RedirectToRoute(
                        "DefaultDetail",
                        new { controller = "Circles", action = "CreateEmailGroup", id = circle.CircleId });
                }
            }

            if (this.ModelState.IsValid)
            {
                 var group = await
                    this.circleService.CreateEmailGroupAsync(
                        model.Name,
                        this.CurrentUser,
                        membersToSendTo,
                        circle, this.PortalUrl);
                this.NotifyUserSuccess(Alert.MessageGroupCreatedSuccess);

                return this.RedirectToRoute(
                    "DefaultDetail",
                    new { controller = "Circles", action = "MessageGroup", id = circle.CircleId, groupId = group.CircleEmailGroupId });
            }
            else
            {
                model.CircleName = circle.Name;
                model.CircleUsers = circle.Members;
                model.CurrentUser = this.CurrentUser;
                model.CheckedUsers = checkedUsersIdList;
                return this.View(string.Format("CreateEmailGroup{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEmail(MessagingGroupModel model, int? id, int? groupId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || !groupId.HasValue)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var group = await this.circleService.GetEmailGroupAsync(groupId.Value);
            if (group == null || !group.Receivers.Any(r => r.Receiver.Id == this.CurrentUser.Id))
            {
                return this.HttpNotFound();
            }

            await this.circleService.CreateGroupEmailMessageAsync(group, model.MessageText, this.CurrentUser, circle, this.PortalUrl);

            return this.RedirectToAction("MessageGroup", "Circles", new { id, groupId });
        }

        [HttpGet]
        public ActionResult CreateMessage(int? id)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            return this.RedirectToAction("Messages", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMessage(int? id, MessagesViewModel model)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                var attachment = ImageHelper.GetResizedImage(
                    model.Attachment,
                    MaxAttachmentImageSize,
                    MaxAttachmentImageSize);
                if (attachment == null && model.Attachment != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    return this.RedirectToAction("Messages", "Circles", new { id });
                }

                await
                    this.circleService.CreateMessageAsync(
                        model.NewMessage,
                        attachment,
                        circle,
                        this.CurrentUser,
                        this.PortalUrl,
                        this.CurrentUser);

                this.NotifyUserSuccess(Alert.CircleMessageCreated);
            }

            return this.RedirectToRoute(
                "DefaultDetail",
                new { controller = "Circles", action = "Messages", id = circle.CircleId });
        }

        [HttpGet]
        public ActionResult CreateMessageReaction(int? id)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            return this.RedirectToAction("Messages", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMessageReaction(int? id, CircleMessagesViewModel model)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                var attachment = ImageHelper.GetResizedImage(
                    model.Attachment,
                    MaxAttachmentImageSize,
                    MaxAttachmentImageSize);

                if (attachment == null && model.Attachment != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    return this.RedirectToAction("Messages", "Circles", new { id });
                }

                var message = await this.circleService.GetMessageByIdAsync(circle.CircleId, model.Message.MessageId);
                if (message == null)
                {
                    return this.HttpNotFound();
                }

                await
                    this.circleService.CreateMessageReactionAsync(model.NewReaction, attachment, circle, message, this.CurrentUser);

                this.NotifyUserSuccess(Alert.CircleMessageReactionCreated);

                return this.RedirectToRoute(
                    "DefaultDetail",
                    new { controller = "Circles", action = "Messages", id = circle.CircleId });
            }

            var messages =
                circle.Messages.Select(
                    message =>
                    new CircleMessagesViewModel
                        {
                            MessageId = message.MessageId,
                            Message = message,
                            CreationDateTime = message.CreationDateTime,
                            IsCreator = message.Creator == this.CurrentUser,
                        }).ToList();
            var viewModel = new MessagesViewModel
                                {
                                    CircleId = circle.CircleId,
                                    CircleIsPrivate = circle.IsPrivate,
                                    CircleName = circle.Name,
                                    Messages = messages
                                };
            return this.View("Messages", viewModel);
        }

        [HttpGet]
        public ActionResult DeleteMessage(int? id)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            return this.RedirectToAction("Messages", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMessage(int circleId, int messageId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);

            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            var message = circle.Messages.FirstOrDefault(m => m.MessageId == messageId);

            if (message == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser)
                || (message.Creator != this.CurrentUser && !this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            await
                this.circleService.DeleteMessageAsync(
                    message,
                    circle);

            this.NotifyUserSuccess(Alert.CircleMessageDeleted);

            return new RedirectResult(this.Url.Action("Messages"));
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            var timeLineItems = new List<DetailViewModel.TimelineItem>();

            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var isMember = this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser);
            if (isMember)
            {
                if (circle.IsPrivate && this.CurrentUserCanOnlyView())
                {
                    return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
                }

                if (this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
                {
                    foreach (var joinRequest in circle.JoinRequests)
                    {
                        timeLineItems.Add(
                            new DetailViewModel.TimelineItem
                            {
                                Action =
                                    string.Format(
                                        Text.OverviewJoinRequest,
                                        joinRequest.User.Name),
                                Date = joinRequest.JoinRequestDateTime,
                                Category = Label.Members,
                                Url =
                                    string.Format(
                                        "{0}/circles/{1}/joinrequests",
                                        ConfigurationManager.AppSettings["PortalUrl"],
                                        circle.CircleId),
                                User = joinRequest.User
                            });
                    }

                    foreach (var memberInvitation in circle.Invitations)
                    {
                        var action = string.Format(
                            Text.OverviewInvited,
                            memberInvitation.InvitedBy.Name,
                            memberInvitation.Email);
                        if (memberInvitation.User != null)
                        {
                            action = string.Format(
                                Text.OverviewInvited,
                                memberInvitation.InvitedBy.Name,
                                memberInvitation.User.Name);
                        }

                        timeLineItems.Add(
                            new DetailViewModel.TimelineItem
                            {
                                Action = action,
                                Date = memberInvitation.InvitationDateTime,
                                Category = Label.Members,
                                Url =
                                    string.Format(
                                        "{0}/circles/{1}/members",
                                        ConfigurationManager.AppSettings["PortalUrl"],
                                        circle.CircleId),
                                User = memberInvitation.User
                            });
                    }
                }

                foreach (var message in circle.Messages)
                {
                    timeLineItems.Add(
                        new DetailViewModel.TimelineItem
                            {
                                Action =
                                    string.Format(
                                        Text.OverviewNewMessage,
                                        message.Creator.Name),
                                Date = message.CreationDateTime,
                                Category = Label.Messages,
                                Url =
                                    string.Format(
                                        "{0}/circles/{1}/messages",
                                        ConfigurationManager.AppSettings["PortalUrl"],
                                        circle.CircleId),
                                User = message.Creator
                            });
                }

                foreach (var job in circle.Jobs)
                {
                    timeLineItems.Add(
                        new DetailViewModel.TimelineItem
                            {
                                Action =
                                    string.Format(
                                        Text.OverviewNewTask,
                                        job.Creator.Name,
                                        job.Title),
                                Date = job.CreationDateTime,
                                Category = Label.Tasks,
                                Url =
                                    string.Format(
                                        "{0}/circles/{1}/jobs",
                                        ConfigurationManager.AppSettings["PortalUrl"],
                                        circle.CircleId),
                                User = job.Creator
                            });
                }

                foreach (var photo in circle.Photos)
                {
                    timeLineItems.Add(
                        new DetailViewModel.TimelineItem
                            {
                                Action =
                                    string.Format(
                                        Text.OverviewNewPhoto,
                                        photo.UploadedBy.Name),
                                Date = photo.Photo.CreationDate,
                                Category = Label.Photos,
                                Url =
                                    string.Format(
                                        "{0}/circles/{1}/documents",
                                        ConfigurationManager.AppSettings["PortalUrl"],
                                        circle.CircleId),
                                User = photo.UploadedBy
                            });
                }

                var activities = this.circleService.GetActivitiesByCircle(circle);
                foreach (var activity in activities)
                {
                    timeLineItems.Add(
                        new DetailViewModel.TimelineItem
                        {
                            Action =
                                    string.Format(
                                        Text.OverviewNewAction,
                                        activity.Creator.Name, activity.Title),
                            Date = activity.CreationDate,
                            Category = Label.Activities,
                            Url =
                                    string.Format(
                                        "{0}/circles/{1}/activities/{2}",
                                        ConfigurationManager.AppSettings["PortalUrl"],
                                        circle.CircleId,
                                        activity.ActivityId),
                            User = activity.Creator
                        });
                }
            }

            var model = new DetailViewModel
                            {
                                TimeLineItems = timeLineItems,
                                Name = circle.Name,
                                CircleId = circle.CircleId,
                                Description = circle.Description,
                                IsPrivate = circle.IsPrivate,
                                IsCurrentUserMember = isMember
            };

            return this.View(string.Format("Detail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [ChildActionOnly]
        public ActionResult DetailHeader(int? id, string selectedTab)
        {
            // No async/await here, because it's not supported by ASP.NET MVC in childactions.
            var circle = this.circleService.GetCircleByIdAsync(id).Result;
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var userMembership = this.circleService.GetCircleMembershipById(circle.CircleId, this.CurrentUser.Id);
            var userIsMemberOfCircle = userMembership != null
                                       && !this.CurrentUserCanOnlyView();
            var userCircleInvitation = this.circleService.GetUserInvitationForCircle(this.CurrentUser, circle);
            if (!userIsMemberOfCircle && circle.IsPrivate && userCircleInvitation == null)
            {
                return this.HttpForbidden();
            }

            var alternativeLabels = this.circleService.GetCircleLabels(circle);
            var settings = this.circleService.GetCircleSettings(circle);

            var altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Info");

            var menuItems = new List<MenuItemModel>
                                {
                                    new MenuItemModel(
                                        "DefaultDetail",
                                        new RouteValueDictionary(
                                        new { id, controller = "Circles", action = "Detail" }),
                                        altLabel != null ? altLabel.Text : Button.Info)
                                };

            if (userIsMemberOfCircle)
            {
                altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Messages");
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Circles", action = "Messages" }),
                        altLabel != null ? altLabel.Text : Button.Messages));

                if (ConfigurationManager.AppSettings["ShowRestyle"] == "true")
                {
                    altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Activities");
                    menuItems.Add(
                        new MenuItemModel(
                            "CircleActivities",
                            new RouteValueDictionary(new { circleId = id, controller = "Activities", action = "Index" }),
                            altLabel != null ? altLabel.Text : Button.Activities,
                            ViewUtils.IsActiveAction(
                                        this.ControllerContext.ParentActionViewContext,
                                        "Activities")
                            ));
                }

                if (!settings.Any(s => s.SettingName == "ShowActivities"))
                {
                    altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Jobs");
                    menuItems.Add(
                        new MenuItemModel(
                            "CircleSubResources",
                            new RouteValueDictionary(new { circleId = id, controller = "Jobs", action = "Index" }),
                            altLabel != null ? altLabel.Text : Button.Jobs));
                }

                altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Members");
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Circles", action = "Members" }),
                        altLabel != null ? altLabel.Text : Button.Members));

                altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Photos");
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Circles", action = "Photos" }),
                        altLabel != null ? altLabel.Text : Button.Photos));

                altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Documents");
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Circles", action = "Documents" }),
                        altLabel != null ? altLabel.Text : Button.Documents));

                altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Marketplace");
                menuItems.Add(
                    new MenuItemModel(
                        "CircleMarketplace",
                        new RouteValueDictionary(new { circleId = id, controller = "Marketplace", action = "Overview" }),
                        altLabel != null ? altLabel.Text : Button.Marketplace,
                        subMenuItems: new List<MenuItemModel>() { new MenuItemModel("CircleMarketplaceDetail", new RouteValueDictionary(new { controller = "Marketplace", action = "Detail" }), null) }
                        ));

                altLabel = alternativeLabels.FirstOrDefault(l => l.Label == "Button.Inbox");
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Circles", action = "Messaging" }),
                        altLabel != null ? altLabel.Text : Button.Inbox));

                // menuItems.Add(
                // new MenuItemModel(
                // "DefaultDetail",
                // new RouteValueDictionary(new { id, controller = "Circles", action = "Agenda" }),
                // Button.Agenda));
            }

            if (!menuItems.Any(m => m.Selected))
            {
                foreach (var item in menuItems)
                {
                    item.Selected = ViewUtils.IsActiveAction(
                        this.ControllerContext.ParentActionViewContext,
                        item.RouteValues["controller"].ToString(),
                        item.RouteValues["action"].ToString())
                            || item.SubMenuItems.Any(subItem => ViewUtils.IsActiveAction(
                                this.ControllerContext.ParentActionViewContext, subItem.RouteValues["controller"].ToString(), subItem.RouteValues["action"].ToString()));

                    if (item.Text == Button.Members)
                    {
                        item.Selected = item.Selected
                                        || ViewUtils.IsActiveAction(
                                            this.ControllerContext.ParentActionViewContext,
                                            item.RouteValues["controller"].ToString(),
                                            "joinrequests");
                    }
                    else if (item.Text == Button.Inbox)
                    {
                        item.Selected = item.Selected
                                        || ViewUtils.IsActiveAction(
                                            this.ControllerContext.ParentActionViewContext,
                                            item.RouteValues["controller"].ToString(),
                                            "InboxSend");
                        item.Selected = item.Selected
                                        || ViewUtils.IsActiveAction(
                                            this.ControllerContext.ParentActionViewContext,
                                            item.RouteValues["controller"].ToString(),
                                            "CreateEmailMessage");
                    }
                }
            }

            if (!string.IsNullOrEmpty(selectedTab))
            {
                var selectItem = menuItems.FirstOrDefault(m => (string)m.RouteValues["action"] == selectedTab);
                if (selectItem != null)
                {
                    selectItem.Selected = true;
                }
            }

            var model = Mapper.Map<DetailHeaderViewModel>(circle);
            model.CurrentUserIsCircleAdministrator = this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser)
                                                     && !this.CurrentUserCanOnlyView();
            model.CurrentUserCanLeaveCircle = this.circleService.IsUserAllowedToLeaveCircle(circle, this.CurrentUser)
                                              && !this.CurrentUserCanOnlyView();
            model.CurrentUserIsMember = this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser)
                                        && !this.CurrentUserCanOnlyView();
            model.CurrentUserHasRequestedToJoin =
                this.CurrentUser.CircleJoinRequests.Any(r => r.Circle.CircleId == circle.CircleId);
            model.MenuItems = menuItems;
            model.CurrentUserCanOnlyView = this.CurrentUserCanOnlyView();
            model.HelpIcons = this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser).Result;
            if (userMembership != null)
            {
                model.ReceiveEmails = userMembership.ReceiveEmails;
            }

            model.CurrentUserIsInvitedToJoin = userCircleInvitation != null && userCircleInvitation.ExpireDate > DateTime.Now;
            if (userCircleInvitation != null)
            {
                model.InvitationToken = userCircleInvitation.AcceptToken;
            }

            return this.PartialView(string.Format("_DetailHeader{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            // TODO: Automapper?
            var model = new EditModel
                            {
                                Circle = circle,
                                Name = circle.Name,
                                IsPrivate = circle.IsPrivate,
                                Description = circle.Description,
                                CoverColors = this.circleService.GetCoverColors(),
                                CoverColor = circle.CoverColor
                            };
            return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, EditModel model)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                var coverColors = this.circleService.GetCoverColors();
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
                    model.Circle = circle;
                    model.CoverColors = this.circleService.GetCoverColors();
                    return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
                }

                var coverImage = ImageHelper.GetResizedImage(model.CoverImage, MaxCoverImageSize, MaxCoverImageSize);
                if (coverImage == null && model.CoverImage != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    model.Circle = circle;
                    model.CoverColors = this.circleService.GetCoverColors();
                    return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
                }

                await
                    this.circleService.EditCircleAsync(
                        circle,
                        model.Name,
                        model.Description,
                        model.IsPrivate,
                        coverColor,
                        profileImage,
                        coverImage);

                return this.RedirectToAction("Detail", new { id = circle.CircleId });
            }

            this.SetStatusCode(HttpStatusCode.BadRequest);
            model.Circle = circle;
            model.CoverColors = this.circleService.GetCoverColors();
            return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public ActionResult EditMessage(int? id)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            return this.RedirectToAction("Messages", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMessage(int circleId, int messageId, string newMessageText)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);

            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            var message = circle.Messages.FirstOrDefault(m => m.MessageId == messageId);

            if (message == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid && !string.IsNullOrEmpty(newMessageText))
            {
                await
                    this.circleService.EditMessageAsync(
                        message,
                        circle,
                        this.CurrentUser,
                        this.PortalUrl,
                        newMessageText);

                this.NotifyUserSuccess(Alert.CircleMessageEdited);
            }

            if (string.IsNullOrEmpty(newMessageText))
            {
                this.NotifyUserDanger(Alert.CircleMessageEditEmpy);
            }

            return new RedirectResult(this.Url.Action("Messages") + "#message-" + messageId);
        }

        [ChildActionOnly]
        public ActionResult FeaturedCircles(string controller)
        {
            var circles = this.circleService.GetFeaturedCircles(this.CurrentUser);
            var model = new FeaturedCirclesModel();
            model.Circles = circles.Select(c => new FeaturedCirclesModel.FeaturedCircle() { CircleId = c.CircleId, CircleName = c.Name, Selected = false }).ToList();
            return this.PartialView("_FeaturedCircles", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GiveAdmin(int? id, string userId)
        {
            var membership = await this.circleService.GetCircleMembershipByIdAsync(id, userId);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(membership.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.GiveAdminRightsAsync(membership);
            this.NotifyUserSuccess(Alert.CircleUserAdminGiven, membership.User.Name);
            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpGet]
        public ActionResult GiveAdmin(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpGet]
        public async Task<ActionResult> InboxReceived(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            var model = new CircleInboxViewModel
                            {
                                CircleId = circle.CircleId,
                                CircleIsPrivate = circle.IsPrivate,
                                CircleName = circle.Name,
                                EmailMessages =
                                    this.circleService.GetEmailMessages(
                                        circle.CircleId,
                                        this.CurrentUser.Id),
                                CurrentUserId = this.CurrentUser.Id
                            };

            return this.View("Inbox", model);
        }

        [HttpGet]
        public async Task<ActionResult> InboxSend(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);

            var model = new CircleInboxSendViewModel
                            {
                                CircleId = circle.CircleId,
                                CircleIsPrivate = circle.IsPrivate,
                                CircleName = circle.Name,
                                SendEmailMessages =
                                    this.circleService.GetSendEmailMessages(
                                        circle.CircleId,
                                        this.CurrentUser.Id),
                                CurrentUserId = this.CurrentUser.Id
                            };

            return this.View("InboxSend", model);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.Circles);

            var circles =
                await this.circleService.GetUserCirclesAsync(this.CurrentUser);

            var model = new IndexViewModel
                            {
                                Circles =
                                    Mapper.Map<List<IndexViewModel.CircleViewModel>>(circles),
                                CurrentUserCanOnlyView = this.CurrentUserCanOnlyView(),
                                MyCircles = true,
                                HelpIcons =
                                    await
                                    this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                        this.CurrentUser)
                            };
            foreach (var circle in model.Circles)
            {
                circle.IsCurrentUserMember = this.CurrentUser.Circles.Any(c => c.Circle.CircleId == circle.CircleId);
                circle.IsCurrentUserAdmin =
                    this.CurrentUser.Circles.Any(c => c.IsAdministrator && c.Circle.CircleId == circle.CircleId);
                circle.MemberCount = circles.First(c => c.CircleId == circle.CircleId).Members.Count;
            }

            var invitations = await this.circleService.GetInvitationsByUserAsync(this.CurrentUser);
            var invitationViews = invitations.Where(i => i.ExpireDate > DateTime.Now).Select(i => new IndexViewModel.CircleInvitationViewModel()
            {
                CircleId = i.Circle.CircleId,
                MemberCount = i.Circle.Members.Count,
                Name = i.Circle.Name,
                ProfileImageId = i.Circle.ProfileImageId,
                Token = i.AcceptToken,
                InvitationId = i.CircleInvitationId
            }).ToList();
            model.InvitationCircles = invitationViews;

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUser(int? id, string userId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            var user = await this.userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            await this.circleService.InviteUserAsync(circle, user, this.CurrentUser, this.PortalUrl);

            return this.Json(new { state = "success", message = String.Format(Alert.UserIsInvitedForTheCircle, user.Name) });
        }

        [HttpGet]
        public ActionResult InviteUser(int? id)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Members", id });
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUsersByEmail(int? id, string emailAddresses)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            var addresses = this.SeparateEmailAddresses(emailAddresses).ToList();
            var filteredAddresses = addresses.Where(x => Utils.IsValidEmail(x)).ToList();
            var filteredEmailAddresses = string.Join(", ", filteredAddresses);
            if (filteredAddresses.Any())
            {
                await
                    this.circleService.InviteUserByEmailAsync(
                        circle,
                        filteredAddresses,
                        this.CurrentUser,
                        this.PortalUrl);

                return this.Json(new { state = "success", message = String.Format(Alert.UsersInvitedForTheCircleByEmail, filteredEmailAddresses) });
            }

            return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
        }

        [HttpGet]
        public ActionResult InviteUsersByEmail(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Members", id });
        }

        [HttpGet]
        public async Task<ActionResult> JoinRequests(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            var model = new JoinRequestsViewModel { CircleId = circle.CircleId, CircleName = circle.Name };
            model.JoinRequests.AddRange(
                Mapper.Map<List<JoinRequestsViewModel.JoinRequestViewModel>>(circle.JoinRequests));

            // Administrators get to see the list of users which have been invited to join the circle.
            if (this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                model.Invitations.AddRange(
                    Mapper.Map<List<MembersViewModel.MemberViewModel>>(
                        await this.circleService.GetInvitationsAsync(circle)));
            }

            return this.View(string.Format("JoinRequests{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Leave(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAllowedToLeaveCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.UserLeaveCircle(circle, this.CurrentUser);
            this.NotifyUserSuccess(Alert.LeaveCircleSuccess);

            return this.RedirectToRoute("Default", new { controller = "Circles", action = "Index" });
        }

        [HttpGet]
        public ActionResult Leave()
        {
            return this.RedirectToRoute("Default", new { controller = "Circles", action = "Index" });
        }

        [HttpGet]
        public async Task<ActionResult> Members(int? id, string query)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var model = new MembersViewModel
                            {
                                CircleId = circle.CircleId,
                                CircleName = circle.Name,
                                CircleIsPrivate = circle.IsPrivate,
                                CurrentUserIsCircleAdministrator =
                                    this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser),
                                HelpIcons =
                                    await
                                    this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                        this.CurrentUser)
                            };

            model.Members.AddRange(Mapper.Map<List<MembersViewModel.MemberViewModel>>(circle.Members));

            // Administrators get to see the list of users which have been invited to join the circle.
            //if (this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            //{
            //    model.Members.AddRange(
            //        Mapper.Map<List<MembersViewModel.MemberViewModel>>(
            //            await this.circleService.GetInvitationsAsync(circle)));
            //}

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                model.Members = model.Members.Where(m => m.UserName.ToLower().Contains(query) || (m.ProfileDescription != null && m.ProfileDescription.ToLower().Contains(query))).ToList();
            }

            model.Members =
                model.Members.OrderByDescending(m => m.IsAdministrator).ThenByDescending(m => m.HasBeenInvited).ToList();

            return this.View(string.Format("Members{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> MemberDetail(int? id, string userId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var membership = circle.Members.FirstOrDefault(m => m.User.Id == userId);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            var model = new MemberDetailModel
            {
                CircleId = circle.CircleId,
                CircleName = circle.Name,
                ProfileText = membership.Profile,
                UserName = membership.User.Name
            };
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Messages(int? id, int? scrollToReaction)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var messages =
                circle.Messages.Where(m => m.IsHidden != true).Select(
                    message =>
                    new CircleMessagesViewModel
                        {
                            MessageId = message.MessageId,
                            Message = message,
                            CreationDateTime = message.CreationDateTime,
                            LatestReactionDateTime = message.Reactions.Any() ? message.Reactions.OrderByDescending(r => r.CreationDateTime).Select(r => r.CreationDateTime).First() : default(DateTime?),
                            IsCreator = message.Creator == this.CurrentUser,
                        }).ToList();
            var model = new MessagesViewModel
                            {
                                CircleId = circle.CircleId,
                                CircleIsPrivate = circle.IsPrivate,
                                CircleName = circle.Name,
                                Messages = messages,
                                ScrollToReactionId = scrollToReaction
                            };

            this.ViewBag.UserIsCircleAdmin = this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser);

            return this.View(string.Format("Messages{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Messaging(int? id)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var groups = await this.circleService.GetEmailGroupsAsync(this.CurrentUser, circle);

            var model = new MessagingViewModel()
            {
                CircleId = circle.CircleId,
                CircleName = circle.Name,
                Groups = groups.Select(g =>
                    new MessagingViewModel.MessagingGroup()
                    {
                        GroupId = g.CircleEmailGroupId,
                        Name = g.Name,
                        Receivers = g.Receivers.Where(r => (r.Receiver != null ? r.Receiver.Id : null) != this.CurrentUser.Id).Select(r => new MessagingViewModel.MessagingGroup.MessageReceiver { Name = r.Receiver != null ? r.Receiver.Name : null, ProfileImageId = r.Receiver != null ? r.Receiver.ProfileImageId : null, UserId = r.Receiver != null ? r.Receiver.Id : null } ).ToList()
                    }
                ).ToList()
            };

            foreach (var group in model.Groups)
            {
                var message = await this.circleService.GetLastEmailMessage(group.GroupId);
                if (message != null)
                {
                    group.LastMessage = message.CreationDateTime;
                }
            }

            model.Groups = model.Groups.OrderByDescending(g => g.LastMessage).ToList();

            return this.View(string.Format("messaging{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),model);
        }

        [HttpGet]
        public async Task<ActionResult> MessageGroup(int? id, int? groupId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || !groupId.HasValue)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var group = await this.circleService.GetEmailGroupAsync(groupId.Value);
            if (group == null || !group.Receivers.Any(r => r.Receiver.Id == this.CurrentUser.Id) || group.Circle.CircleId != circle.CircleId)
            {
                return this.HttpNotFound();
            }

            var messages = await this.circleService.GetEmailGroupMessagesAsync(group);

            var model = new MessagingGroupModel()
            {
                CircleId = circle.CircleId,
                CircleName = circle.Name,
                CurrentUserId = this.CurrentUser.Id,
                GroupId = group.CircleEmailGroupId,
                Name = group.Name,
                Receivers = group.Receivers.Where(r => (r.Receiver != null ? r.Receiver.Id : null) != this.CurrentUser.Id)
                    .Select(r => new MessagingGroupModel.MessageReceiver { Name = (r.Receiver != null ? r.Receiver.Name : null), ProfileImageId = (r.Receiver != null ? r.Receiver.ProfileImageId : null), UserId = r.Receiver != null ? r.Receiver.Id : null }).ToList(),
                Messages = messages.Select(m => new MessagingGroupModel.Message { CreationDate = m.CreationDateTime, Text = m.Text, UserId = m.Creator.Id, UserName = m.Creator.Name }).ToList()
            };
            return this.View(string.Format("MessageGroup{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> OpenEmailMessage(int? emailMessageId)
        {
            var emailMessage = await this.circleService.GetEmailMessageAsync(emailMessageId);
            return this.View("EmailMessageDetail", new CircleEmailMessageDetailModel { EmailMessage = emailMessage, });
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> Photo(int? id, int? mediaId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || circle.Members.All(m => m.User.Id != this.CurrentUser.Id))
            {
                return this.HttpNotFound();
            }

            var photo = await this.circleService.GetPhotoByMediaId(circle.CircleId, mediaId);
            if (photo == null)
            {
                return this.HttpNotFound();
            }

            return this.ResizedPhoto(photo.Data);
        }

        [HttpGet]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> PhotoLarge(int? id, int? mediaId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || circle.Members.All(m => m.User.Id != this.CurrentUser.Id))
            {
                return this.HttpNotFound();
            }

            var photo = await this.circleService.GetPhotoByMediaId(circle.CircleId, mediaId);
            if (photo == null)
            {
                return this.HttpNotFound();
            }

            return this.ResizedPhoto(photo.Data, false);
        }

        [HttpGet]
        public async Task<ActionResult> Photos(int? id, int? photoAlbumId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var albums = new List<CirclePhotosViewModel.CircleAlbumViewModel>();
            foreach (var circlePhotoAlbum in circle.CirclePhotoAlbums)
            {
                var cover = circlePhotoAlbum.Photos.FirstOrDefault();
                albums.Add(
                    new CirclePhotosViewModel.CircleAlbumViewModel
                    {
                        Id = circlePhotoAlbum.CirclePhotoAlbumId,
                        Title = circlePhotoAlbum.Title,
                        Count = circlePhotoAlbum.Photos.Count,
                        Cover = cover
                    });
            }

            var model = new CirclePhotosViewModel
            {
                CircleId = circle.CircleId,
                CircleName = circle.Name,
                PhotoAlbums = albums,
                CurrentUserIsCircleAdministrator =
                                    this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser)
                                    && !this.CurrentUserCanOnlyView()
            };

            var album = circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == photoAlbumId);
            if (album != null)
            {
                var items = await this.circleService.GetAllCirclePhotosForAlbumAsync(album.CirclePhotoAlbumId);
                model.Documents = items.Select(p => new CirclePhotosViewModel.OwnedMedia(p.Photo, p.UploadedBy.Id)).Where(c => string.IsNullOrEmpty(c.Item.FontAwesomeClass())).ToList();
                model.CurrentPhotoAlbum =
                    circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == album.CirclePhotoAlbumId);
            }
            else
            {
                var circlePhotos = await this.circleService.GetAllCirclePhotosFromCircleAsync(circle.CircleId);
                var documents = circlePhotos.Select(p => new CirclePhotosViewModel.OwnedMedia(p.Photo, p.UploadedBy.Id));
                var messages = await this.circleService.GetAllMediaFromCircleMessagesAsync(id);

                model.Documents = documents.Union(messages.Select(m => new CirclePhotosViewModel.OwnedMedia(m, string.Empty))).Where(c => string.IsNullOrEmpty(c.Item.FontAwesomeClass())).ToList();
            }

            model.CurrentUserId = this.CurrentUser.Id;

            return this.View(string.Format("Photos{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Documents(int? id, int? photoAlbumId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var albums = new List<CirclePhotosViewModel.CircleAlbumViewModel>();
            foreach (var circlePhotoAlbum in circle.CirclePhotoAlbums)
            {
                var cover = circlePhotoAlbum.Photos.FirstOrDefault();
                albums.Add(
                    new CirclePhotosViewModel.CircleAlbumViewModel
                        {
                            Id = circlePhotoAlbum.CirclePhotoAlbumId,
                            Title = circlePhotoAlbum.Title,
                            Count = circlePhotoAlbum.Photos.Count,
                            Cover = cover
                        });
            }

            var model = new CirclePhotosViewModel
                            {
                                CircleId = circle.CircleId,
                                CircleName = circle.Name,
                                PhotoAlbums = albums,
                                CurrentUserIsCircleAdministrator =
                                    this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser)
                                    && !this.CurrentUserCanOnlyView(),
                            };

            var album = circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == photoAlbumId);
            if (album != null)
            {
                var items = await this.circleService.GetAllCirclePhotosForAlbumAsync(album.CirclePhotoAlbumId);
                model.Documents = items.Select(p => new CirclePhotosViewModel.OwnedMedia(p.Photo, p.UploadedBy.Id)).Where(c => !string.IsNullOrEmpty(c.Item.FontAwesomeClass())).ToList();
                model.CurrentPhotoAlbum =
                    circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == album.CirclePhotoAlbumId);
            }
            else
            {
                var circlePhotos = await this.circleService.GetAllCirclePhotosFromCircleAsync(circle.CircleId);
                var documents = circlePhotos.Select(p => new CirclePhotosViewModel.OwnedMedia(p.Photo, p.UploadedBy.Id));

                model.Documents = documents.Where(c => !string.IsNullOrEmpty(c.Item.FontAwesomeClass())).ToList();
            }

            model.CurrentUserId = this.CurrentUser.Id;

            return this.View(string.Format("Documents{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadDocument(int? id, int? mediaId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var files = await this.circleService.GetAllMediaForCircleAsync(id);
            if (files == null)
            {
                return this.HttpNotFound();
            }

            var file = files.FirstOrDefault(m => m.MediaId == mediaId);
            if (file == null)
            {
                return this.HttpNotFound();
            }

            return this.File(file.Data, file.MimeType, file.Name);
        }

        [HttpGet]
        [AllowAnonymous]
        //[OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> ProfileImage(int? id, int? mediaId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (circle.IsPrivate && !this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpNotFound();
            }

            var profileImage = await this.circleService.GetProfileImageByCircleIdAsync(id, mediaId);
            if (profileImage == null)
            {
                return this.HttpNotFound();
            }

            return this.ResizedProfileImage(profileImage.Data);
        }

        [HttpGet]
        public async Task<ActionResult> Public()
        {
            this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.PublicCircles);

            var circles = await
                this.circleService.GetPublicCirclesInNeighborhoodAsync(
                    this.CurrentUser.Id,
                    this.CurrentUser.Position,
                    this.CurrentUserNeighborhoodRadius);

            var model = new IndexViewModel
                            {
                                Circles =
                                    Mapper.Map<List<IndexViewModel.CircleViewModel>>(circles),
                                CurrentUserCanOnlyView = this.CurrentUserCanOnlyView(),
                                MyCircles = false,
                                HelpIcons =
                                    await
                                    this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                        this.CurrentUser)
                            };

            foreach (var circle in model.Circles)
            {
                circle.HasRequestedToJoin =
                    this.CurrentUser.CircleJoinRequests.Any(r => r.Circle.CircleId == circle.CircleId);
                circle.IsInvitedToJoin =
                    this.CurrentUser.CircleInvitations.Any(r => r.Circle.CircleId == circle.CircleId);
                circle.IsCurrentUserMember = this.CurrentUser.Circles.Any(c => c.Circle.CircleId == circle.CircleId);
                circle.HelpIcons = await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser);
                circle.MemberCount = circles.First(c => c.CircleId == circle.CircleId).Members.Count();
            }

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> RejectInvitation(int? id, string code)
        {
            var invitation = await this.circleService.GetInvitationByCircleIdAcceptTokenAsync(id, code);

            if (invitation != null && invitation.User == null)
            {
                invitation.User = await this.userService.FindByEmailAsync(this.CurrentUser.Email);
            }

            if (invitation == null || invitation.User == null || this.CurrentUser.Id != invitation.User.Id)
            {
                this.NotifyUserDanger(Error.AccountDoesntExist);
                return this.RedirectToAction("Index", "Home");
            }

            // Right user but to late
            if (DateTime.Now > invitation.ExpireDate && invitation.User.Id == this.CurrentUser.Id)
            {
                this.NotifyUserDanger(Error.InvitationExpired);
                return this.RedirectToAction("Index", "Home");
            }


            var message = string.Format(Common.InterfaceText.Notification.CircleInvitationDeclined, invitation.User.Name, invitation.Circle.Name);
            await this.notificationService.CreateNotificationForUserAsync(invitation.InvitedBy, message, null, SettingName.CircleInvitationDeclined);

            this.NotifyUserSuccess(Alert.InvitationRejected, invitation.Circle.Name);
            await this.circleService.RemoveInvitationAsync(invitation.Circle.CircleId, this.CurrentUser.Id);

            return this.RedirectToAction("Index", "Circles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RejectJoinRequest(int? id, int? requestId)
        {
            var joinRequest = await this.circleService.GetJoinRequestByIdAsync(id, requestId);
            if (joinRequest == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(joinRequest.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.RejectJoinRequestAsync(joinRequest);
            this.NotifyUserSuccess(Alert.JoinRequestRejected);

            return this.RedirectToAction("JoinRequests", "Circles", new { id });
        }

        [HttpGet]
        public ActionResult RejectJoinRequest(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("JoinRequests", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.RemoveCircleAsync(circle);

            this.NotifyUserSuccess(Alert.RemoveCircleSuccess);

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Index" });
        }

        [HttpPost]
        public async Task<ActionResult> RemoveAttachment(int circleId, int messageId, int? reactionId, int attachmentId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);

            var message = circle.Messages.FirstOrDefault(m => m.MessageId == messageId);

            if (message == null)
            {
                return this.HttpNotFound();
            }

            CircleMessageReaction reaction = null;
            if (reactionId.HasValue)
            {
                reaction = message.Reactions.FirstOrDefault(r => r.ReactionId == reactionId);
                if (reaction == null)
                {
                    return this.HttpNotFound();
                }

                await this.circleService.RemoveAttachment(reaction, attachmentId);
            }
            else
            {
                await this.circleService.RemoveAttachment(message, attachmentId);
            }

            return new RedirectResult(this.Url.Action("Messages") + "#message-" + messageId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveCirclePhoto(int? circleId, int photoId, int? photoAlbumId, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.RemoveCirclePhoto(photoId);
            this.NotifyUserSuccess(Alert.CirclePhotoRemoved);

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", circleId, photoAlbumId });
        }

        [HttpGet]
        public ActionResult RemoveCirclePhoto(int circleId, int? photoAlbumId)
        {
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Photos", photoAlbumId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveCircleDocument(int? circleId, int photoId, int? photoAlbumId, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.RemoveCirclePhoto(photoId);
            this.NotifyUserSuccess(Alert.CircleDocumentRemoved);

            return this.RedirectToRoute("DefaultDetail", new { action = "Documents", controller = "Circles", circleId, photoAlbumId });
        }

        [HttpGet]
        public ActionResult RemoveCircleDocument(int circleId, int? photoAlbumId)
        {
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Documents", photoAlbumId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveEmailGroup(int? id, int? groupId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView() || !groupId.HasValue)
            {
                return this.HttpNotFound();
            }

            var group = await this.circleService.GetEmailGroupAsync(groupId.Value);
            if (group == null)
            {
                return this.HttpNotFound();
            }

            var receiver = group.Receivers.FirstOrDefault(r => r.Receiver.Id == this.CurrentUser.Id);
            if (receiver == null)
            {
                return this.HttpNotFound();
            }

            await this.circleService.RemoveEmailGroupReceiver(receiver);
            this.NotifyUserSuccess(Alert.CircleEmailGroupRemoved);

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Messaging", id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveMember(int? id, string memberId, string email)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(memberId))
                {
                    await this.circleService.RemoveMemberAsync(circle.CircleId, memberId);
                }
                else
                {
                    await this.circleService.RemoveInvitationByEmailAsync(circle.CircleId, email);
                }

                this.NotifyUserSuccess(Alert.RemoveMemberFromCircleSuccess);
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id, action = "Members" });
        }

        [HttpGet]
        public ActionResult RemoveMember(int? id)
        {
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id, action = "Members" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhotoAlbum(int? circleId, int photoAlbumId, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var album = circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == photoAlbumId);
            if (album == null)
            {
                return this.HttpNotFound();
            }

            await this.circleService.RemoveCirclePhotoAlbumAsync(photoAlbumId);

            this.NotifyUserSuccess(string.Format(Alert.CirclePhotoAlbumRemoved, album.Title));

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Documents" });
        }

        [HttpGet]
        public ActionResult RemovePhotoAlbum(int circleId, int photoAlbumId)
        {
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", action = "Documents" });
        }

        public async Task<ActionResult> RemoveReceivedEmailMessage(int circleId, string userId, int emailMessageId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            await this.circleService.RemoveReceivedCircleEmailMessageAsync(userId, emailMessageId);
            this.NotifyUserSuccess(Alert.MessageRemovedFromInbox);

            return this.RedirectToAction("InboxReceived", "Circles", circleId);
        }

        public async Task<ActionResult> RemoveSendEmailMessage(int circleId, string userId, int emailMessageId)
        {
            var circle = await this.circleService.GetCircleActivitiesByIdAsync(circleId);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (this.CurrentUserCanOnlyView())
            {
                return this.View(string.Format("ProfileNotAllowed{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
            }

            await this.circleService.RemoveSendCircleEmailMessageAsync(userId, emailMessageId);
            this.NotifyUserSuccess(Alert.MessageRemovedFromSend);

            return this.RedirectToAction("InboxSend", "Circles", circleId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestToJoin(int? id, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            var result = await this.circleService.RequestToJoinAsync(circle, this.CurrentUser, this.PortalUrl);
            if (result)
            {
                this.NotifyUserSuccess(Alert.AskToJoinCircleSuccess);
            }

            return this.RedirectToAction("Detail", "Circles", new { id });
        }

        [HttpGet]
        public ActionResult RequestToJoin(int? id)
        {
            return this.RedirectToAction("Detail", "Circles", new { id });
        }

        //[HttpGet]
        //public async Task<ActionResult> ReactToPrivateMessage(int id, int messageId, bool reactToAll)
        //{
        //    var circle = await this.circleService.GetCircleByIdAsync(id);
        //    if (circle == null )
        //    {
        //        return this.HttpNotFound();
        //    }
        //    var message = await this.circleService.GetEmailMessageAsync(messageId);
        //    if (message == null || !(message.Recipients.Any(r => r.Receiver.Id == this.CurrentUser.Id)))
        //    {
        //        return this.HttpNotFound();
        //    }

        //    var subject = message.Subject;
        //    if (!subject.StartsWith("RE: "))
        //        subject = "RE: " + subject;

        //    var checkedUsers = new List<string>();
        //    if (reactToAll)
        //    {
        //        checkedUsers = message.Recipients.Select(r => r.Receiver.Id).ToList();
        //    }
        //    checkedUsers.Add(message.Creator.Id);

        //    var model = new CircleCreateEmailMessageModel
        //    {
        //        CircleId = circle.CircleId,
        //        CircleName = circle.Name,
        //        CircleUsers = circle.Members,
        //        CurrentUser = this.CurrentUser,
        //        subjectText = subject,
        //        CheckedUsers = checkedUsers
        //    };

        //    return this.View("CreateEmailMessage", model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RevokeAdmin(int? id, string userId)
        {
            var membership = await this.circleService.GetCircleMembershipByIdAsync(id, userId);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(membership.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.RevokeAdminRightsAsync(membership);
            this.NotifyUserSuccess(Alert.CircleUserAdminRevoked, membership.User.Name);
            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpGet]
        public ActionResult RevokeAdmin(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpGet]
        public ActionResult SendInvitationReminder(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendInvitationReminder(int? id, string memberId, string email)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            var user = await this.userService.GetUserByIdAsync(memberId);

            if (circle == null || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(memberId))
                {
                    await this.circleService.SendInvitationReminderAsync(circle, user, this.CurrentUser, this.PortalUrl);
                }
                else
                {
                    await this.circleService.SendInvitationReminderByEmailAsync(circle, email, this.CurrentUser, this.PortalUrl);
                }

                this.NotifyUserSuccess(String.Format(Alert.UserIsInvitedForTheCircle, user != null ? user.UserName : email));
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id, action = "Members" });
        }

        public async Task<ActionResult> SearchMembers(int? id, string q, List<int> except = null)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            var circle = await this.circleService.GetCircleByIdAsync(id.Value);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var users =
                Mapper.Map<List<SearchMemberViewModel>>(
                    await this.circleService.SearchMembersAsync(circle, q, new List<int>()));
            foreach (var user in users.Where(u => u.HasProfileImage))
            {
                user.ProfileImageUrl = this.Url.RouteUrl(
                    "UserProfileImage",
                    new
                        {
                            controller = "Users",
                            action = "ProfileImage",
                            userId = user.UserId,
                            mediaId = user.UserProfileImageId
                        });
            }

            return this.Json(users, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SearchUsers(int? id, string q)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            var circle = await this.circleService.GetCircleByIdAsync(id.Value);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var users =
                Mapper.Map<List<SearchUserViewModel>>(
                    await
                    this.circleService.SearchNeighborsNotInCircleAsync(
                        circle,
                        this.CurrentUserPosition,
                        this.CurrentUserNeighborhoodRadius,
                        q));
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
        public async Task<ActionResult> SetPrivate(int? id, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || circle.IsPrivate || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.SetPrivateAsync(circle.CircleId);
            this.NotifyUserSuccess(Alert.SetCirclePrivateSuccess);

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id });
        }

        [HttpGet]
        public ActionResult SetPrivate(int? id)
        {
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id });
        }

        [HttpGet]
        public ActionResult SetPublic(int? id)
        {
            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPublic(int? id, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(id);
            if (circle == null || !circle.IsPrivate || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.SetPublicAsync(circle.CircleId);
            this.NotifyUserSuccess(Alert.SetCirclePublicSuccess);

            return this.RedirectToRoute("DefaultDetail", new { controller = "Circles", id });
        }

        private Media PostedFileToMedia(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.IsImage())
            {
                return this.mediaService.CreateMedia(ImageHelper.GetResizedImage(file, MaxPhotoImageSize, MaxPhotoImageSize));
            }

            return this.mediaService.CreateMedia(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StopNotifications(int? id)
        {
            var membership = await this.circleService.GetCircleMembershipByIdAsync(id, this.CurrentUser.Id);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            await this.circleService.StopCircleEmailAsync(membership);
            this.NotifyUserSuccess(Alert.CircleUserEmailStopped, membership.User.Name);
            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StartNotifications(int? id)
        {
            var membership = await this.circleService.GetCircleMembershipByIdAsync(id, this.CurrentUser.Id);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            await this.circleService.StartCircleEmailAsync(membership);
            this.NotifyUserSuccess(Alert.CircleUserEmailStarted, membership.User.Name);
            return this.RedirectToAction("Members", "Circles", new { id });
        }
    }
}