// <copyright file="UserSetting.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class UserSetting
    {
        public int UserSettingId { get; set; }

        public int SettingId { get; set; }

        public string UserId { get; set; }

        public string Value { get; set; }
    }
}
