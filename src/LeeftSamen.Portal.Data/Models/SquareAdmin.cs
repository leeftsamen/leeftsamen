// <copyright file="SquareAdmin.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class SquareAdmin
    {
        public virtual int SquareAdminId { get; set; }

        public virtual Square Square { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual User CreatedBy { get; set; }
    }
}
