// <copyright file="ChangeNotificationViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Account
{
    public class ChangeNotificationViewModel
    {
        public class Setting
        {
            public int SettingId { get; set; }

            public string Group { get; set; }

            public string Name { get; set; }

            public string Text { get; set; }

            public string DefaultValue { get; set; }
        }

        public class SettingValue
        {
            public int SettingId { get; set; }

            public string Value { get; set; }
        }

        public List<string> Names { get; set; }

        public List<Setting> Settings { get; set; }

        public List<SettingValue> Values { get; set; }
    }
}