// <copyright file="NotificationsController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Net;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoMapper;

    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Notifications;
    using LeeftSamen.Portal.Web.Utils;
    using LeeftSamen.Common.Extensions;

    /// <summary>
    /// The notifications controller.
    /// </summary>
    public class NotificationsController : BaseController
    {
        /// <summary>
        /// The notification service.
        /// </summary>
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsController"/> class.
        /// </summary>
        /// <param name="currentUserInformation">
        /// The current user information.
        /// </param>
        /// <param name="notificationService">
        /// The notification Service.
        /// </param>
        public NotificationsController(
            ICurrentUserInformation currentUserInformation,
            INotificationService notificationService)
            : base(currentUserInformation)
        {
            this.notificationService = notificationService;
        }

        /// <summary>
        /// The get new notification.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> GetNewNotification(bool? stripped)
        {
            var notification = await this.notificationService.GetNewNotificationForUserAsync(this.CurrentUser);

            NotificationViewModel model = null;
            if (notification != null)
            {
                model = Mapper.Map<NotificationViewModel>(notification);
                await this.notificationService.SetNotificationShownAsync(notification);
            }

            if (stripped != null && model != null && stripped == true)
            {
                model.Message = model.Message.StripHtmlAndNormalize();
            }

            return model == null
                       ? this.Json(new { }, JsonRequestBehavior.AllowGet)
                       : this.Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetNewNotificationStripped()
        {
            return await this.GetNewNotification(true);
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
            var notifications =
                                    await
                                    this.notificationService.GetLatestNotificationsForUserAsync(
                                        this.CurrentUser,
                                        100);
            var model = new IndexViewModel { Notifications = Mapper.Map<List<NotificationViewModel>>(notifications) };
            return this.View(model);
        }

        /// <summary>
        /// The redirect and set read.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> RedirectAndSetRead(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            var notification = await this.notificationService.GetNotificationByIdAsync(id.Value);
            if (notification == null || notification.ForUserId != this.CurrentUser.Id)
            {
                return this.HttpNotFound();
            }

            await this.notificationService.SetNotificationReadAsync(notification);

            if (notification.Url != null)
            {
                return this.Redirect(notification.Url);
            }

            return this.RedirectToAction("Index", "Notifications");
        }

        [HttpGet]
        public async Task<HttpStatusCode> SetAllRead()
        {
            await this.notificationService.SetAllNotificationsReadAsync(this.CurrentUser);

            return HttpStatusCode.OK;
        }
    }
}