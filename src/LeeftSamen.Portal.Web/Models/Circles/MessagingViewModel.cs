// <copyright file="MessagingViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class MessagingViewModel
    {
        public int CircleId { get; set; }

        public string CircleName { get; set; }

        public List<MessagingGroup> Groups { get; set; }

        public class MessagingGroup
        {
            public int GroupId { get; set; }

            public string Name { get; set; }

            public DateTime? LastMessage { get; set; }

            public List<MessageReceiver> Receivers { get; set; }

            public class MessageReceiver
            {
                public int? ProfileImageId { get; set; }

                public string UserId { get; set; }

                public string Name { get; set; }
            }
        }
    }
}