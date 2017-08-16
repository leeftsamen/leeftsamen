// <copyright file="FeaturedCirclesModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class FeaturedCirclesModel
    {
        public List<FeaturedCircle> Circles { get; set; }

        public class FeaturedCircle
        {
            public int CircleId { get; set; }

            public string CircleName { get; set; }

            public bool Selected { get; set; }
        }
    }
}