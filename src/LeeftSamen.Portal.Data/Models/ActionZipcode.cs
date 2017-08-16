// <copyright file="ActionZipcode.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class ActionZipcode
    {
        public virtual int ActionZipcodeId { get; set; }

        public virtual Action Action { get; set; }

        public virtual string PostalCode { get; set; }
    }
}
