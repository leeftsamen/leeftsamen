// <copyright file="TownViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    public class TownViewModel
    {
        public string Name { get; set; }

        public List<DistrictViewModel> Districts { get; set; }
    }
}