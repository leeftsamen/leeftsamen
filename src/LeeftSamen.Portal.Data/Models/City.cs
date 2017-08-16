// <copyright file="City.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System.Data.Entity.Spatial;

    public class City
    {
        public virtual string CityCode { get; set; }

        public virtual int CityId { get; set; }

        public virtual string CityName { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual DbGeography Shape { get; set; }
    }
}