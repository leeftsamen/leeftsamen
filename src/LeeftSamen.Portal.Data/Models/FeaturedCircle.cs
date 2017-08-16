// <copyright file="FeaturedCircle.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class FeaturedCircle
    {
        public virtual int FeaturedCircleId { get; set; }

        public virtual Circle Circle { get; set; }
    }
}
