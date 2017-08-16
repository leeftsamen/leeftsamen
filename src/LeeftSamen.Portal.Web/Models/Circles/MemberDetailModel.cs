// <copyright file="MemberDetailModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class MemberDetailModel
    {
        public int CircleId { get; set; }

        public string CircleName { get; set; }

        public string UserName { get; set; }

        public string ProfileText { get; set; }
    }
}