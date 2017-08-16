// <copyright file="DetailViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class DetailViewModel
    {
        public int SquareId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}