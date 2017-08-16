// <copyright file="SquareFact.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class SquareFact
    {
        public virtual int SquareFactId { get; set; }

        public virtual Square Square { get; set; }

        public virtual string Title { get; set; }

        public virtual string IntroductionText { get; set; }

        public virtual string FullText { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual User Creator { get; set; }

        public List<SquareFactMedia> MediaList { get; set; }
    }
}
