// <copyright file="ActivityAttendance.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class ActivityAttendance
    {
        public virtual int ActivityAttendanceId { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual bool Attending { get; set; }

        public virtual DateTime? ModificationDate { get; set; }

        public virtual User User { get; set; }
    }
}