// <copyright file="SquareFactMedia.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class SquareFactMedia
    {
        public int SquareFactMediaId { get; set; }

        public SquareFact SquareFact { get; set; }

        public Media Media { get; set; }
    }
}
