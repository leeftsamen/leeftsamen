// <copyright file="CircleJoinRequest.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class CircleJoinRequest
    {
        public virtual string AcceptToken { get; set; }

        public virtual Circle Circle { get; set; }

        public virtual int CircleJoinRequestId { get; set; }

        public virtual DateTime JoinRequestDateTime { get; set; }

        public virtual User User { get; set; }
    }
}