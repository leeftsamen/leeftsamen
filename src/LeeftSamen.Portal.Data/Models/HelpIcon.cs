// <copyright file="HelpIcon.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System.Collections.Generic;

    public class HelpIcon
    {
        public virtual int HelpIconId { get; set; }

        public virtual string Text { get; set; }

        public virtual string TextPlacement { get; set; }

        public virtual string Type { get; set; }

        public virtual ICollection<HelpIconUser> ShownIcons { get; set; }
    }
}
