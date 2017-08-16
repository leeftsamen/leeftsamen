// <copyright file="AttendanceFormViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Activities
{
    public class AttendanceFormViewModel
    {
        public AttendanceFormViewModel(int activityId, int? circleId, bool isAttending, bool redirectToDetail = false)
        {
            this.ActivityId = activityId;
            this.CircleId = circleId;
            this.IsAttending = isAttending;
            this.RedirectToDetail = redirectToDetail;
        }

        public int ActivityId { get; set; }

        public int? CircleId { get; set; }

        public bool IsAttending { get; set; }

        public bool RedirectToDetail { get; set; }
    }
}