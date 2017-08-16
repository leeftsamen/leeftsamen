// <copyright file="CircleEmailGroup.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CircleEmailGroup
    {
        public int CircleEmailGroupId { get; set; }

        public Circle Circle { get; set; }

        public string Name { get; set; }

        public User Creator { get; set; }

        public List<CircleEmailMessageReceiver> Receivers { get; set; }
    }
}
