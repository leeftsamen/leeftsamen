// <copyright file="EditPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;

    public class EditPostModel
    {
        public EditPostModel()
        {
            this.ActiveInDistricts = new List<int>();
            this.ActiveOrganizationThemes = new List<int>();
            this.Services = new List<ServiceViewModel>();
            this.Products = new List<ProductViewModel>();
        }

        public List<ProductViewModel> Products { get; set; }

        public List<ServiceViewModel> Services { get; set; }

        public List<int> ActiveInDistricts { get; set; }

        public List<int> ActiveOrganizationThemes { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Address")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "AddressIsRequired")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Label), Name = "OpeningHours")]
        public string OpeningHours { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Town")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TownIsRequired")]
        public string City { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        public string Description { get; set; }

        [EmailAddress]
        [Display(ResourceType = typeof(Label), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsRequired")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ChooseLogo")]
        public HttpPostedFileBase Logo { get; set; }

        [Display(ResourceType = typeof(Label), Name = "OrganizationName")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "OrganizationNameIsRequired")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Label), Name = "OrganizationType")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "OrganizationTypeIsRequired")]
        public OrganizationType.Types OrganizationTypeType { get; set; }

        [Display(ResourceType = typeof(Label), Name = "PhoneNumber")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PhoneNumberIsRequired")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Label), Name = "PostalCode")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PostalCodeIsRequired")]
        public string PostalCode { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Website")]
        //[RegularExpression("^https?://.*", ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "WebsiteFormat")]
        public string Website { get; set; }
    }
}