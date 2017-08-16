// <copyright file="CircleActivityReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class CircleActivityReaction
    {
        public virtual int ReactionId { get; set; }

        public virtual int CircleId { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual CircleActivity CircleActivity { get; set; }

        public virtual string Text { get; set; }
    }
}
