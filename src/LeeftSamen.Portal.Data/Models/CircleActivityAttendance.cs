// <copyright file="CircleActivityAttendance.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class CircleActivityAttendance
    {
        public virtual int ActivityAttendanceId { get; set; }

        public virtual CircleActivity CircleActivity { get; set; }

        public virtual bool Attending { get; set; }

        public virtual DateTime? UserJoinedDate { get; set; }

        public virtual User User { get; set; }
    }
}
