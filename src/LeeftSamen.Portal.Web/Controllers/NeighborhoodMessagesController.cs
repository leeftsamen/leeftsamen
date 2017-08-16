// <copyright file="NeighborhoodMessagesController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.UI;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Attributes;
    using LeeftSamen.Portal.Web.Extensions;
    using LeeftSamen.Portal.Web.Helpers;
    using LeeftSamen.Portal.Web.Models.NeighborhoodMessages;
    using LeeftSamen.Portal.Web.Utils;
    using System.Web;
    using System;    /// <summary>
                     /// The neighborhood messages controller.
                     /// </summary>
    [CurrentUserMustBeInActiveCity]
    public class NeighborhoodMessagesController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NeighborhoodMessagesController"/> class.
        /// </summary>
        /// <param name="currentUserInformation">
        /// The current User Information.
        /// </param>
        /// <param name="neighborhoodMessageService">
        /// The neighborhood Service.
        /// </param>
        /// <param name="helpIconService">
        /// </param>
        public NeighborhoodMessagesController(
            ICurrentUserInformation currentUserInformation,
            INeighborhoodMessageService neighborhoodMessageService,
            IMediaService mediaService,
            ISharedService sharedService,
            IHelpIconService helpIconService)
            : base(currentUserInformation)
        {
            this.neighborhoodMessageService = neighborhoodMessageService;
            this.mediaService = mediaService;
            this.helpIconService = helpIconService;
            this.sharedService = sharedService;
        }

        private readonly IHelpIconService helpIconService;

        /// <summary>
        /// The neighborhood message service.
        /// </summary>
        private readonly INeighborhoodMessageService neighborhoodMessageService;

        private readonly IMediaService mediaService;

        private readonly ISharedService sharedService;

        private const int MaxPhotoImageSize = 600;

        /// <summary>
        /// The association messages.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> AssociationMessages()
        {
            return await this.IndexView(NeighborhoodMessage.MessageTypes.AssociationMessages);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public ActionResult CreateMessage()
        {
            if (!this.CurrentUserCanCreateMessage())
            {
                return this.HttpNotFound();
            }

            var model = new PostMessageViewModel() { AllowSharing = true };

            return this.View(string.Format("EditMessage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <param name="postedModel">
        /// The posted model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMessage(PostMessageViewModel postedModel)
        {
            if (!this.CurrentUserCanCreateMessage())
            {
                return this.HttpNotFound();
            }

            var isValidImage = postedModel.Image1.IsImage(true) && postedModel.Image2.IsImage(true) && postedModel.Image3.IsImage(true) && postedModel.Image4.IsImage(true) && postedModel.Image5.IsImage(true);
            var file1 = this.PostedFileToMedia(postedModel.File1);
            var isValidFile = postedModel.File1.IsImage(true) || !string.IsNullOrEmpty(file1.FontAwesomeClass());
            if (!this.ModelState.IsValid || !isValidImage)
            {
                if (!isValidImage)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                }

                return this.View(string.Format("EditMessage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postedModel);
            }

            if (file1 != null && !isValidFile)
            {
                this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                return this.View(string.Format("EditMessage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postedModel);
            }

            if (postedModel.ExpirationDateTime.HasValue)
            {
                postedModel.ExpirationDateTime = postedModel.ExpirationDateTime.Value.AddHours(postedModel.ExpirationDateTimeHour).AddMinutes(postedModel.ExpirationDateTimeMinute);
            }

            var image1 = ImageHelper.GetResizedImage(postedModel.Image1, 600, 600);
            var image2 = ImageHelper.GetResizedImage(postedModel.Image2, 600, 600);
            var image3 = ImageHelper.GetResizedImage(postedModel.Image3, 600, 600);
            var image4 = ImageHelper.GetResizedImage(postedModel.Image4, 600, 600);
            var image5 = ImageHelper.GetResizedImage(postedModel.Image5, 600, 600);

            var message =
                await
                this.neighborhoodMessageService.CreateNeighborhoodMessageAsync(
                    postedModel.Title,
                    postedModel.IntroductionText,
                    postedModel.FullText,
                    this.CurrentUser,
                    this.CurrentUserInformation.OrganizationMembership,
                    postedModel.Expires ? postedModel.ExpirationDateTime : null,
                    image1,
                    image2,
                    image3,
                    image4,
                    image5,
                    file1,
                    postedModel.AllowSharing);

            var type = NeighborhoodMessage.MessageTypes.NeighborMessages;
            if (this.CurrentUserInformation.OrganizationMembership != null)
            {
                switch (this.CurrentUserInformation.OrganizationMembership.Organization.OrganizationType.Type)
                {
                    case OrganizationType.Types.Professional:
                    case OrganizationType.Types.Volunteer:
                        type = NeighborhoodMessage.MessageTypes.OrganizationMessages;
                        break;
                    case OrganizationType.Types.Association:
                        type = NeighborhoodMessage.MessageTypes.AssociationMessages;
                        break;
                }
            }

            return this.RedirectToRoute(
                "NeighborhoodMessage",
                new { action = "MessageDetail", messageId = message.MessageId, messageType = type });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(int? messageId, int? mediaId)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return this.HttpNotFound();
            }

            if (message.File1.MediaId != mediaId)
            {
                return this.HttpNotFound();
            }

            return this.File(message.File1.Data, message.File1.MimeType, message.File1.Name);
        }

        /// <summary>
        /// The edit message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> EditMessage(int? messageId, NeighborhoodMessage.MessageTypes? messageType)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null || !this.CurrentUserCanEditMessage(message))
            {
                return this.HttpNotFound();
            }

            var model = Mapper.Map<PostMessageViewModel>(message);
            model.Expires = message.ExpirationDateTime.HasValue;
            model.ExpirationDateTimeHour = message.ExpirationDateTime.HasValue
                                               ? message.ExpirationDateTime.Value.Hour
                                               : 0;
            model.ExpirationDateTimeMinute = message.ExpirationDateTime.HasValue
                                                 ? message.ExpirationDateTime.Value.Minute
                                                 : 0;
            return this.View(string.Format("EditMessage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The edit message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="postedModel">
        /// The posted model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMessage(
            int? messageId,
            NeighborhoodMessage.MessageTypes? messageType,
            PostMessageViewModel postedModel)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null || !this.CurrentUserCanEditMessage(message))
            {
                return this.HttpNotFound();
            }

            var file1 = this.PostedFileToMedia(postedModel.File1);
            var isValidFile = postedModel.File1.IsImage(true) || !string.IsNullOrEmpty(file1.FontAwesomeClass());

            if (this.ModelState.IsValid && (file1 == null || isValidFile))
            {
                if (postedModel.ExpirationDateTime.HasValue)
                {
                    postedModel.ExpirationDateTime =
                        postedModel.ExpirationDateTime.Value.AddHours(postedModel.ExpirationDateTimeHour)
                            .AddMinutes(postedModel.ExpirationDateTimeMinute);
                }

                var image1 = ImageHelper.GetResizedImage(postedModel.Image1, 600, 600);
                var image2 = ImageHelper.GetResizedImage(postedModel.Image2, 600, 600);
                var image3 = ImageHelper.GetResizedImage(postedModel.Image3, 600, 600);
                var image4 = ImageHelper.GetResizedImage(postedModel.Image4, 600, 600);
                var image5 = ImageHelper.GetResizedImage(postedModel.Image5, 600, 600);

                await
                    this.neighborhoodMessageService.UpdateNeighborhoodMessageAsync(
                        message,
                        postedModel.Title,
                        postedModel.IntroductionText,
                        postedModel.FullText,
                        postedModel.Expires ? postedModel.ExpirationDateTime : null,
                        image1,
                        image2,
                        image3,
                        image4,
                        image5,
                        file1,
                        postedModel.AllowSharing);

                return this.RedirectToRoute("NeighborhoodMessage", new { messageId = message.MessageId, messageType });
            }

            if (!isValidFile)
            {
                this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
            }

            var model = Mapper.Map<PostMessageViewModel>(message);
            model.Expires = message.ExpirationDateTime.HasValue;
            model.ExpirationDateTimeHour = message.ExpirationDateTime.HasValue
                                               ? message.ExpirationDateTime.Value.Hour
                                               : 0;
            model.ExpirationDateTimeMinute = message.ExpirationDateTime.HasValue
                                                 ? message.ExpirationDateTime.Value.Minute
                                                 : 0;
            return this.View(string.Format("EditMessage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return await this.IndexView(NeighborhoodMessage.MessageTypes.Any);
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> Image(int? messageId, int? mediaId)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return this.HttpNotFound();
            }

            if (message.File1 == null || message.File1Id != mediaId)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(message.File1.Data);
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
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "crop;width;height")]
        public async Task<ActionResult> ItemImage(int? id, int mediaId, int? index, int height = 600, int width = 600, bool crop = false)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(id);
            if (message == null)
            {
                return this.HttpNotFound();
            }

            Media itemImage = null;
            switch (index)
            {
                case 1:
                    itemImage = message.Image1;
                    break;
                case 2:
                    itemImage = message.Image2;
                    break;
                case 3:
                    itemImage = message.Image3;
                    break;
                case 4:
                    itemImage = message.Image4;
                    break;
                case 5:
                    itemImage = message.Image5;
                    break;
            }

            if (itemImage == null || itemImage.MediaId != mediaId)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(itemImage.Data);
            image = image.Resize(height, width, true, true);

            if (crop)
            {
                if (image.Height < height)
                {
                    decimal calc = (decimal)height / (decimal)image.Height;
                    image = image.Resize(Convert.ToInt32(Math.Ceiling((decimal)image.Width * calc)), height);
                }

                if (image.Width < width)
                {
                    decimal calc = (decimal)width / (decimal)image.Width;
                    image = image.Resize(width, Convert.ToInt32(Math.Ceiling((decimal)image.Height * calc)));
                }

                int cropVert = (image.Height - height) / 2;
                int cropHor = (image.Width - width) / 2;
                cropVert = cropVert > 1 ? cropVert : 0;
                cropHor = cropHor > 1 ? cropHor : 0;
                image = image.Crop(cropVert, cropHor, cropVert, cropHor);
            }
            else
            {
                // We make sure the height is never larger than 2 times the width
                int cropMargin = image.Height - (image.Width * 2);
                if (cropMargin > 1)
                {
                    image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);
                }
            }

            image = image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        /// <summary>
        /// The message detail.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageType">
        /// The message Type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> MessageDetail(int? messageId, NeighborhoodMessage.MessageTypes? messageType)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return this.HttpNotFound();
            }

            this.ViewBag.UserPosition = this.CurrentUserPosition;

            var model = new MessageDetailViewModel
                            {
                                Message = message,
                                MessageType = messageType ?? NeighborhoodMessage.MessageTypes.Any,
                                UserCanEditMessage = this.CurrentUserCanEditMessage(message),
                                Reactions = message.Reactions.OrderByDescending(r => r.CreationDateTime).ToList(),
                                UserCanPinMessage = this.CurrentUser.Roles.Any(r => r.RoleId == "Admin")
                            };
            return this.View(string.Format("MessageDetail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PinMessage(int messageId, bool pin)
        {
            if (this.CurrentUser.Roles.All(r => r.RoleId != "Admin"))
            {
                return this.RedirectToAction("MessageDetail", new { messageId });
            }

            await this.neighborhoodMessageService.PinMessageAsync(messageId, pin);

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateReaction(int? messageId, NeighborhoodMessage.MessageTypes? messageType, ReactionPostModel model)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                await this.neighborhoodMessageService.CreateReactionAsync(message, this.CurrentUser, this.CurrentUserInformation.OrganizationMembership, model.NewReaction);

                this.NotifyUserSuccess(Alert.NeighborhoodMessageReactionCreated);
            }

            return this.RedirectToAction("MessageDetail", new { messageId, messageType });
        }

        /// <summary>
        /// The remove reaction.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageType">
        /// The message Type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveReaction(int? messageId, int? reactionId, NeighborhoodMessage.MessageTypes? messageType)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null || !this.CurrentUserCanEditMessage(message))
            {
                return this.HttpNotFound();
            }

            var reaction = message.Reactions.Where(r => r.ReactionId == reactionId).FirstOrDefault();
            if (reaction == null)
            {
                return this.HttpNotFound();
            }

            await this.neighborhoodMessageService.RemoveReactionAsync(reaction);

            return this.RedirectToAction("MessageDetail", new { messageId, messageType });
        }

        /// <summary>
        /// The message image.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="mediaId">
        /// The media id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> MessageImage(int? messageId, int? mediaId)
        {
            var messageImage = await this.neighborhoodMessageService.GetMessageImageAsync(messageId, mediaId);
            if (messageImage == null)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(messageImage.Data);
            image.Resize(600, 600);

            // We make sure the height is never larger than 2 times the width
            int cropMargin = image.Height - (image.Width * 2);
            if (cropMargin > 1)
            {
                image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);
            }

            image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        /// <summary>
        /// The neighbor messages.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> NeighborMessages()
        {
            return await this.IndexView(NeighborhoodMessage.MessageTypes.NeighborMessages);
        }

        /// <summary>
        /// The organization messages.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> OrganizationMessages()
        {
            return await this.IndexView(NeighborhoodMessage.MessageTypes.OrganizationMessages);
        }

        /// <summary>
        /// The remove message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageType">
        /// The message Type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveMessage(int? messageId, NeighborhoodMessage.MessageTypes? messageType)
        {
            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null || !this.CurrentUserCanEditMessage(message))
            {
                return this.HttpNotFound();
            }

            await this.neighborhoodMessageService.RemoveNeighborhoodMessageAsync(message);

            return this.RedirectToAction(messageType.ToString(), "NeighborhoodMessages");
        }

        /// <summary>
        /// The remove message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult RemoveMessage(int? messageId)
        {
            return this.RedirectToAction("Index", "NeighborhoodMessages");
        }

        /// <summary>
        /// The current user can create message.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CurrentUserCanCreateMessage()
        {
            return true;
        }

        /// <summary>
        /// The current user can edit message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CurrentUserCanEditMessage(NeighborhoodMessage message)
        {
            return (this.CurrentUserInformation.OrganizationMembership != null
                    && message.OrganizationMembershipId.HasValue
                    && this.CurrentUserInformation.OrganizationMembership.Organization.OrganizationId
                    == message.OrganizationMembership.Organization.OrganizationId
                    && this.CurrentUserInformation.OrganizationMembership.IsAdministrator)
                   || (this.CurrentUserInformation.OrganizationMembership == null
                       && !message.OrganizationMembershipId.HasValue && message.Creator.Id == this.CurrentUser.Id)
                   || this.CurrentUser.Roles.Any(r => r.RoleId == "Admin");
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<ActionResult> IndexView(NeighborhoodMessage.MessageTypes messageType)
        {
            this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.NeighborhoodMessages);
            var messages = new List<NeighborhoodMessage>();
            switch (messageType)
            {
                case NeighborhoodMessage.MessageTypes.Any:
                    messages =
                        await
                        this.neighborhoodMessageService.GetAllMessagesAsync(this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius);
                    break;
                case NeighborhoodMessage.MessageTypes.NeighborMessages:
                    messages =
                        await
                        this.neighborhoodMessageService.GetAllNeighborMessagesAsync(
                            this.CurrentUserPosition,
                            this.CurrentUserNeighborhoodRadius);
                    break;
                case NeighborhoodMessage.MessageTypes.OrganizationMessages:
                    messages =
                        await
                        this.neighborhoodMessageService.GetAllOrganizationMessagesAsync(
                            this.CurrentUserPosition,
                            this.CurrentUserNeighborhoodRadius);
                    break;
                case NeighborhoodMessage.MessageTypes.AssociationMessages:
                    messages =
                        await
                        this.neighborhoodMessageService.GetAllAssociationMessagesAsync(
                            this.CurrentUserPosition,
                            this.CurrentUserNeighborhoodRadius);
                    break;
            }

            var helpIcons = await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser);
            var model = new IndexViewModel
                            {
                                Messages = messages,
                                MessageType = messageType,
                                UserCanCreateMessage = this.CurrentUserCanCreateMessage(),
                                HelpIcons = helpIcons
                            };

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
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
    }
}