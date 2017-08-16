// <copyright file="InviteModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Users
{
    public class InviteModel
    {
        [Display(ResourceType = typeof(Label), Name = "Emails")]
        public string EmailAdresses { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Message")]
        public string EmailText { get; set; }
    }
}