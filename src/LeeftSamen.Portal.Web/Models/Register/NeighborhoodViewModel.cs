// <copyright file="NeighborhoodViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Register
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The neighborhood view model.
    /// </summary>
    public class NeighborhoodViewModel
    {
        public const int DefaultRadius = 2000;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether first try.
        /// </summary>
        public bool FirstTry { get; set; }

        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "HouseNumberIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "HouseNumber")]
        public string HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [Required]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [Required]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the neighborhood radius.
        /// </summary>
        [Required]
        [Range(50, 15000)]
        [DefaultValue(DefaultRadius)]
        public int NeighborhoodRadius { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PostalCodeIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "PostalCode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        [Required]
        public string Street { get; set; }

        public bool HasError { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ShowLocation")]
        public bool ShowLocation { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Address")]
        public string Address { get; set; }
    }
}