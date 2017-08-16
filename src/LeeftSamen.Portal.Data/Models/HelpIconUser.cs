// <copyright file="HelpIconUser.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    public class HelpIconUser
    {
        public virtual int Id { get; set; }

        public virtual HelpIcon HelpIcon { get; set; }

        public virtual User User { get; set; }
    }
}
