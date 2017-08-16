// <copyright file="MemberSearchModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class MemberSearchModel
    {
        public int? circleId { get; set; }

        public string Query { get; set; }
    }
}