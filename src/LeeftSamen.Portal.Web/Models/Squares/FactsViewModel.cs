// <copyright file="FactsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeeftSamen.Portal.Data.Models;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class FactsViewModel
    {
        public int SquareId { get; set; }

        public List<FactModel> Facts { get; set; }

        public bool UserIsAdmin { get; set; }

        public class FactModel
        {
            public int FactId { get; set; }

            public Media OverviewImage { get; set; }

            public string Title { get; set; }

            public string IntroductionText { get; set; }

            public DateTime CreationDate { get; set; }

            public User Creator { get; set; }
        }
    }
}