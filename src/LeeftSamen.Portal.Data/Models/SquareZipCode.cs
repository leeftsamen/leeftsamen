// <copyright file="SquareZipCode.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class SquareZipCode
    {
        public virtual int SquareZipCodeId { get; set; }

        public virtual Square Square { get; set; }

        public virtual string ZipCode { get; set; }
    }
}
