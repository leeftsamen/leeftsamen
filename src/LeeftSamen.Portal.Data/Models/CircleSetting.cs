﻿// <copyright file="CircleSetting.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CircleSetting
    {
        public int CircleSettingId { get; set; }

        public Circle Circle { get; set; }

        public string SettingName { get; set; }

        public string Value { get; set; }
    }
}
