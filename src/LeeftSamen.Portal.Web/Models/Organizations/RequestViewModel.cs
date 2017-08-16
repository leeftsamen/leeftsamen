// <copyright file="RequestViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;

    /// <summary>
    /// The request view model.
    /// </summary>
    public class RequestViewModel : EditPostModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestViewModel"/> class.
        /// </summary>
        public RequestViewModel()
        {
            this.DistrictsInCity = new List<DistrictViewModel>();
            this.OrganizationThemes = new List<OrganizationThemeViewModel>();
        }

        /// <summary>
        /// Gets or sets the districts in city.
        /// </summary>
        public List<DistrictViewModel> DistrictsInCity { get; set; }

        public List<OrganizationThemeViewModel> OrganizationThemes { get; set; }

        /// <summary>
        /// The from post model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="EditViewModel"/>.
        /// </returns>
        public static RequestViewModel FromPostModel(EditPostModel model)
        {
            return new RequestViewModel
                       {
                           Address = model.Address,
                           City = model.City,
                           Description = model.Description,
                           Email = model.Email,
                           Logo = model.Logo,
                           Name = model.Name,
                           Phone = model.Phone,
                           PostalCode = model.PostalCode,
                           Website = model.Website,
                           ActiveInDistricts = model.ActiveInDistricts,
                           OrganizationTypeType = model.OrganizationTypeType,
                           ActiveOrganizationThemes = model.ActiveOrganizationThemes
                       };
        }
    }
}