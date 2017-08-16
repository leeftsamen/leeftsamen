// <copyright file="RouteConfig.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web
{
    using Data.Enums;
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The route config.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "ForumNoId",
                "{type}/{typeId}/forum/{action}",
                new { action = "Index", controller = "Forums" },
                new { type = getModelTypes(), typeId = "[0-9]+" });

            routes.MapRoute(
                "Forum",
                "{type}/{typeId}/forum/{action}/{id}",
                new { action = "Index", controller = "Forums", id = UrlParameter.Optional },
                new { type = getModelTypes(), typeId = "[0-9]+", id = "[0-9]+" });

            //routes.MapRoute(
            //    "Image",
            //    "{type}/{typeId}/Images/Normal/{id}",
            //    new { action = "Image", controller = "Media" },
            //    new { type = getModelTypes(), typeId = "[0-9]+", id = "[0-9]+" });

            //routes.MapRoute(
            //    "ImageFull",
            //    "{type}/{typeId}/Images/Full/{id}",
            //    new { action = "ImageFull", controller = "Media" },
            //    new { type = getModelTypes(), typeId = "[0-9]+", id = "[0-9]+" });

            routes.MapRoute(
                "CircleMemberDetail",
                "circles/{id}/members/{userId}/",
                new { action = "MemberDetail", controller = "Circles" },
                new { id = "[0-9]+" });

            routes.MapRoute(
                "CircleSubResourcesDetail",
                "circles/{circleId}/jobs/{id}/{action}",
                new { action = "Detail", controller = "Jobs" },
                new { circleId = "[0-9]+", id = "[0-9]+" });

            routes.MapRoute(
                "CircleMarketplace",
                "circles/{circleId}/marketplace",
                new { action = "Overview", controller = "Marketplace" },
                new { circleId = "[0-9]+" });

            routes.MapRoute(
                "CircleMarketplaceDetail",
                "circles/{circleId}/marketplace/{id}",
                new { action = "Detail", controller = "Marketplace" },
                new { circleId = "[0-9]+", id = "[0-9]+" });

            routes.MapRoute(
                "CircleActivities",
                "circles/{circleId}/activities",
                new { action = "Index", controller = "Activities" },
                new { circleId = "[0-9]+" });

            routes.MapRoute(
                "CircleMyActivities",
                "circles/{circleId}/myactivities",
                new { action = "MyActivities", controller = "Activities" },
                new { circleId = "[0-9]+" });

            routes.MapRoute(
                "CircleActivitiesDetail",
                "circles/{circleId}/activities/{id}",
                new { action = "Detail", controller = "Activities" },
                new { circleId = "[0-9]+", id = "[0-9]+" });

            //routes.MapRoute(
            //"SquareForumSubject",
            //"squares/{id}/forum/{subjectId}",
            //new { action = "ForumSubject", controller = "Squares" },
            //new { id = "[0-9]+", subjectId = "[0-9]+" });

#warning Results in 404 on home/jobs
            routes.MapRoute(
                "CircleSubResources",
                "circles/{circleId}/jobs/{action}",
                new { action = "Index", controller = "Jobs" },
                new { circleId = "[0-9]+" });

            routes.MapRoute(
                "NeighborhoodMessage",
                "neighborhoodmessages/{messageType}/{messageId}/{action}/{mediaId}",
                new { action = "MessageDetail", controller = "NeighborhoodMessages", mediaId = UrlParameter.Optional },
                new { messageId = "[0-9]+", messageType = "NeighborMessages|OrganizationMessages|AssociationMessages" });

            routes.MapRoute(
                "SquareFactImage",
                "squareFacts/{factId}/Files/{mediaId}",
                new { action = "FactImage", controller = "Squares", factId = UrlParameter.Optional, mediaId = UrlParameter.Optional },
                new { factId = "[0-9]+", mediaId = "[0-9]+" });

            routes.MapRoute(
                "NeighborhoodMessageImage",
                "neighborhoodmessages/{messageId}/Files/{mediaId}",
                new { action = "Image", controller = "NeighborhoodMessages", messageId = UrlParameter.Optional, mediaId = UrlParameter.Optional },
                new { messageId = "[0-9]+", mediaId = "[0-9]+" });

            //routes.MapRoute(
            //    "ForumReactionImage",
            //    "ForumReactions/{reactionId}/Files/{mediaId}",
            //    new { action = "ReactionImage", controller = "Forums", reactionId = UrlParameter.Optional, mediaId = UrlParameter.Optional },
            //    new { reactionId = "[0-9]+", mediaId = "[0-9]+" });

            //routes.MapRoute(
            //    "ForumReactionImageLarge",
            //    "ForumReactions/{reactionId}/FilesLarge/{mediaId}",
            //    new { action = "ReactionImageLarge", controller = "Forums", reactionId = UrlParameter.Optional, mediaId = UrlParameter.Optional },
            //    new { reactionId = "[0-9]+", mediaId = "[0-9]+" });

            routes.MapRoute("UserInvite", "users/invite", new { action = "invite", controller = "Users" });
            routes.MapRoute("CurrentUserProfileImage", "users/current/profileimage", new { action = "CurrentUserProfileImage", controller = "Users" });
            routes.MapRoute("UserProfileImage", "users/{userId}/profileimage/{mediaId}", new { action = "ProfileImage", controller = "Users" });
            routes.MapRoute("UserProfile", "users/{userId}/{action}", new { action = "Index", controller = "Users" });

            routes.MapRoute(
                "OrganizationMessage",
                "Organization/{id}/Messages/{messageId}",
                new { controller = "Organizations", action = "Message" },
                new { id = "[0-9]+", messageId = "[0-9]+" });

            routes.MapRoute(
                "OrganizationActivity",
                "Organization/{id}/Activities/{activityId}",
                new { controller = "Organizations", action = "Activity" },
                new { id = "[0-9]+", activityId = "[0-9]+" });

            routes.MapRoute(
                "DefaultDetail",
                "{controller}/{id}/{action}/{mediaId}",
                new { action = "Detail", mediaId = UrlParameter.Optional },
                new { id = "[0-9]+" });

            routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" });
        }

        private static string getModelTypes()
        {
            var types = Enum.GetNames(typeof(ModelType));
            return string.Join("|", types);
        }
    }
}