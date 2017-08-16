// <copyright file="BaseController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Web.Helpers;
    using LeeftSamen.Portal.Web.Utils;
    using System.Diagnostics;

    public abstract class BaseController : Controller
    {
        protected BaseController(ICurrentUserInformation currentUserInformation)
        {
            this.CurrentUserInformation = currentUserInformation;
            this.CurrentUserInformation.UpdateLastSeen();

            if(System.Web.HttpContext.Current.Request.QueryString["iospush"] != null)
            {
                if(this.CurrentUserInformation.User != null)
                {
                    this.CurrentUserInformation.AddDevice(System.Web.HttpContext.Current.Request.QueryString["iospush"], DeviceType.iOS);
                }
                else
                {
                    System.Web.HttpContext.Current.Session["registeriosafterlogon"] = System.Web.HttpContext.Current.Request.QueryString["iospush"];
                }
            }
        }

        public bool IsUserProfileValid()
        {
            return this.CurrentUserInformation != null && this.CurrentUserInformation.IsUserProfileValid;
        }

        protected User CurrentUser
        {
            get
            {
                return this.CurrentUserInformation.User;
            }
        }

        protected ICurrentUserInformation CurrentUserInformation { get; private set; }

        protected DbGeography CurrentUserPosition
        {
            get
            {
                return this.CurrentUserInformation.UserPosition;
            }
        }

        protected int CurrentUserNeighborhoodRadius
        {
            get
            {
                return this.CurrentUserInformation.UserNeighborhoodRadius;
            }
        }

        protected string PortalUrl
        {
            get
            {
                return this.Url.Action("Index", "Home", null, this.RequestUrlScheme);
            }
        }

        protected string RequestUrlScheme
        {
            get
            {
                return this.Request.Url != null ? this.Request.Url.Scheme : "https";
            }
        }

        [NonAction]
        public ActionResult FeatureNotAvailableInCity()
        {
            return this.View("FeatureNotAvailableInCity");
        }

        [NonAction]
        protected virtual bool CurrentUserCanOnlyView()
        {
            return this.CurrentUserInformation.OrganizationMembership != null;
        }

        [NonAction]
        protected virtual HttpStatusCodeResult HttpForbidden(string statusDescription = null)
        {
            if (statusDescription != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, statusDescription);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [NonAction]
        protected virtual void NotifyUser(TempDataHelper.NotificationType type, string message, params object[] args)
        {
            this.TempData = TempDataHelper.CreateNotification(type, string.Format(message, args));
        }

        [NonAction]
        protected virtual void NotifyUserDanger(string message, params object[] args)
        {
            this.NotifyUser(TempDataHelper.NotificationType.Danger, message, args);
        }

        [NonAction]
        protected virtual void NotifyUserSuccess(string message, params object[] args)
        {
            this.NotifyUser(TempDataHelper.NotificationType.Success, message, args);
        }

        [NonAction]
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }

        [NonAction]
        protected virtual ActionResult RedirectToStartPage()
        {
            return this.RedirectToAction("Index", "Home");
        }

        [NonAction]
        protected ActionResult ResizedAttachment(byte[] data)
        {
            return this.ResizedImage(data, 246f, false);
        }

        [NonAction]
        protected ActionResult ResizedImage(byte[] data, float size, bool cropSquare = true)
        {
            var image = new WebImage(data);
            if (cropSquare)
            {
                var minSize = (float)Math.Min(image.Width, image.Height);
                var factor = size / minSize;
                image.Resize((int)(image.Width * factor), (int)(image.Height * factor));
                var hc = (int)((image.Width - size) * 0.5);
                var vc = (int)((image.Height - size) * 0.5);
                image.Crop(vc, hc, vc, hc);
                image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            }
            else
            {
                var maxSize = (float)Math.Max(image.Width, image.Height);
                var factor = size / maxSize;
                image.Resize((int)(image.Width * factor), (int)(image.Height * factor));
            }

            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [NonAction]
        protected ActionResult ResizedPhoto(byte[] data, bool cropSquare = true)
        {
            return this.ResizedImage(data, cropSquare ? 180f : 600f, cropSquare);
        }

        [NonAction]
        protected ActionResult ResizedProfileImage(byte[] data)
        {
            return this.ResizedImage(data, 96f);
        }

        [NonAction]
        protected IEnumerable<string> SeparateEmailAddresses(string emailAddresses)
        {
            return emailAddresses.Split().SelectMany(s => s.Split(',', ';',' ','|').Select(e => e.Trim()));
        }

        [NonAction]
        protected virtual void SetStatusCode(HttpStatusCode statusCode)
        {
            this.SetStatusCode((int)statusCode);
        }

        [NonAction]
        protected virtual void SetStatusCode(int statusCode)
        {
            this.Response.StatusCode = statusCode;
            this.Response.TrySkipIisCustomErrors = true;
        }
    }
}