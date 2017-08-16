// <copyright file="CircleAgendaViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System.Collections.Generic;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class CircleAgendaViewModel
    {
        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether circle is private.
        /// </summary>
        public bool CircleIsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets a value if the current user is the admin of the circle
        /// </summary>
        public bool CurrentUserIsCircleAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the circle activities
        /// </summary>
        public List<CircleActivity> CircleActivities { get; set; }
    }
}
