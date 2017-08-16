// <copyright file="MediaController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.Data.Models;
using LeeftSamen.Portal.Services;
using LeeftSamen.Portal.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace LeeftSamen.Portal.Web.Controllers
{
    public class MediaController : BaseController
    {
        private readonly ISquareService squareService;
        private readonly ICircleService circleService;
        private readonly IForumService forumService;
        private readonly IMediaService mediaService;

        public MediaController(
            ICurrentUserInformation currentUserInformation,
            ISquareService squareService,
            ICircleService circleService,
            IForumService forumService,
            IMediaService mediaService)
            : base(currentUserInformation)
        {
            this.squareService = squareService;
            this.circleService = circleService;
            this.forumService = forumService;
            this.mediaService = mediaService;
        }

        public async Task<bool> IsMediaAllowed(string type, int? typeId)
        {
            ModelType typeEnum;
            if (!Enum.TryParse<ModelType>(type, true, out typeEnum))
            {
                return false;
            }

            switch (typeEnum)
            {
                case ModelType.ForumReactions:
                    return await this.IsReactionAllowed(typeId);
            }

            return false;
        }

        private async Task<bool> IsReactionAllowed(int? typeId)
        {
            var reaction = await this.forumService.GetReactionByIdAsync(typeId);
            if (reaction == null)
            {
                return false;
            }

            ModelType typeEnum;
            if (!Enum.TryParse<ModelType>(reaction.Subject.Type, true, out typeEnum))
            {
                return false;
            }

            switch (typeEnum)
            {
                case ModelType.Squares:
                    return await this.squareService.GetSquareByIdAsync(reaction.Subject.TypeId, this.CurrentUser) == null;
                case ModelType.Circles:
                    return await this.circleService.GetCircleByIdAsync(reaction.Subject.TypeId, this.CurrentUser) == null;
            }

            return false;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> Image(string type, int? typeId, int? id)
        {
            if (!id.HasValue || !await this.IsMediaAllowed(type, typeId))
            {
                return this.HttpNotFound();
            }

            Media media = await this.mediaService.GetByIdAsync(id.Value);
            if (media == null)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(media.Data);
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

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> ImageFull(string type, int? typeId, int? id)
        {
            if (!id.HasValue || !await this.IsMediaAllowed(type, typeId))
            {
                return this.HttpNotFound();
            }

            Media media = await this.mediaService.GetByIdAsync(id.Value);
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
    }
}