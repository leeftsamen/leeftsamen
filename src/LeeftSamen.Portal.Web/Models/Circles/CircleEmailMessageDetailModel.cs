// <copyright file="CircleEmailMessageDetailModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System.Collections.Generic;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class CircleEmailMessageDetailModel
    {
        /// <summary>
        /// Gets or sets the emailMessage that the user is currently viewing
        /// </summary>
        public CircleEmailMessage EmailMessage { get; set; }
    }
}
