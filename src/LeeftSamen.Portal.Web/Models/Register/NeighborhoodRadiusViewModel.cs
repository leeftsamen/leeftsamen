// <copyright file="NeighborhoodRadiusViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Register
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class NeighborhoodRadiusViewModel
    {
        private readonly int[] radiusSteps = { 25, 50, 100, 200, 500, 1000, 2000, 3000, 5000 };

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        [Required]
        [Range(50, 15000)]
        [DefaultValue(NeighborhoodViewModel.DefaultRadius)]
        public int NeighborhoodRadius { get; set; }

        public int[] RadiusSteps
        {
            get
            {
                return this.radiusSteps;
            }
        }
    }
}