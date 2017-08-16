// <copyright file="FactDetailModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeeftSamen.Portal.Data.Models;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class FactDetailModel
    {
        public int SquareId { get; set; }

        public int FactId { get; set; }

        public bool UserIsAdmin { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public double Distance { get; set; }

        public DateTime CreationDate { get; set; }

        public User Creator { get; set; }

        public List<Media> Files { get; set; }
    }
}