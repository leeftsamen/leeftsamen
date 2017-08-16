// <copyright file="IndexAuthenticatedViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Home
{
    using Data.Models;
    using System;
    using System.Collections.Generic;

    public class IndexAuthenticatedViewModel
    {
        public string CurrentUserName { get; set; }

        public List<TimelineItem> Timeline { get; set; }

        public string UserId { get; set; }

        public int? UserProfileImageId { get; set; }

        public List<ActionItem> Actions { get; set; }

        public class TimelineItem
        {
            public string Action { get; set; }

            public string Category { get; set; }

            public string CategoryClass { get; set; }

            public DateTime Date { get; set; }

            public string Url { get; set; }

            public int? UserProfileImageId { get; set; }

            public string UserId { get; set; }
        }

        public class NeighborhoodMessageTimelineItem : TimelineItem
        {
            public int MessageId { get; set; }

            public NeighborhoodMessage.MessageTypes MessageType { get; set; }

            public int? ImageId { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }
        }

        public class ActivityTimelineItem : TimelineItem
        {
            public int ActivityId { get; set; }

            public bool Attending { get; set; }
        }

        public class JobTimelineItem : TimelineItem
        {
            public int JobId { get; set; }

            public int CircleId { get; set; }

            public bool Assigned { get; set; }
        }

        public class MarketplaceTimelineItem : TimelineItem
        {
            public int MarketplaceItemId { get; set; }

            public int? ImageId { get; set; }
        }

        public class ActionItem
        {
            public int ActionId { get; set; }

            public string Name { get; set; }

            public string Title { get; set; }

            public string Text { get; set; }
        }
    }
}