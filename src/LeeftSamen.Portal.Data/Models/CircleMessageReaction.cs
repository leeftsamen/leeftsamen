// <copyright file="CircleMessageReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class CircleMessageReaction
    {
        public virtual Circle Circle { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual CircleMessage Message { get; set; }

        public virtual int ReactionId { get; set; }

        public virtual string ReactionText { get; set; }

        public virtual Media Attachment { get; set; }
    }
}