// <copyright file="Setting.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class Setting
    {
        public int SettingId { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string DefaultValue { get; set; }
    }
}
