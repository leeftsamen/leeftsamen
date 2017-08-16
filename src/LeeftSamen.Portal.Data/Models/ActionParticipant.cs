// <copyright file="ActionParticipant.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class ActionParticipant
    {
        public virtual int ActionParticipantId { get; set; }

        public virtual Action Action { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
