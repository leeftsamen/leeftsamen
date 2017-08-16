// <copyright file="MessagingGroupModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class MessagingGroupModel
    {
        public int CircleId { get; set; }

        public string CircleName { get; set; }

        public int GroupId { get; set; }

        public string Name { get; set; }

        public string CurrentUserId { get; set; }

        public List<MessageReceiver> Receivers { get; set; }

        public List<Message> Messages { get; set; }

        public string MessageText { get; set; }

        public class Message
        {
            public string Text { get; set; }

            public string UserId { get; set; }

            public string UserName { get; set; }

            public DateTime CreationDate { get; set; }
        }

        public class MessageReceiver
        {
            public int? ProfileImageId { get; set; }

            public string UserId { get; set; }

            public string Name { get; set; }
        }
    }
}