// <copyright file="AddressViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Register
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    public class AddressViewModel
    {
        [Required]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "HouseNumberIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "HouseNumber")]
        public string HouseNumber { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PostalCodeIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "PostalCode")]
        public string PostalCode { get; set; }

        [Required]
        public string Street { get; set; }

        public bool HasError { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Address")]
        public string Address { get; set; }
    }
}