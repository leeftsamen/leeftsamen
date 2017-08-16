// <copyright file="MenuViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Account
{
    public class MenuViewModel
    {
        public int NeigborhoodMessages { get; set; }

        public int Activities { get; set; }

        public int ForSale { get; set; }

        public int ToBorrow { get; set; }

        public int Meals { get; set; }

        public int NeighborHelp { get; set; }

        public int Organisations { get; set; }

        public int Circles { get; set; }

        public int PublicCircles { get; set; }
    }
}