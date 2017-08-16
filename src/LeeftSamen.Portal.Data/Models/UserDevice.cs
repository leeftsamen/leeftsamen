// <copyright file="UserDevice.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using LeeftSamen.Portal.Data.Enums;

namespace LeeftSamen.Portal.Data.Models
{
    public class UserDevice
    {
        public int UserDeviceId { get; set; }

        public virtual User User { get; set; }

        public string Token { get; set; }

        public DateTime LastUseDate { get; set; }

        public DeviceType DeviceType { get; set; }
    }
}