// <copyright file="EditViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    public class EditViewModel : EditPostModel
    {
        public EditViewModel()
        {
            this.DistrictsInCity = new List<DistrictViewModel>();
            this.OrganizationThemes = new List<OrganizationThemeViewModel>();
        }

        public List<DistrictViewModel> DistrictsInCity { get; set; }

        public int? LogoId { get; set; }

        public List<OrganizationThemeViewModel> OrganizationThemes { get; set; }

        public int OrganizationId { get; set; }

        public static EditViewModel FromPostModel(EditPostModel model, Organization organization)
        {
            return new EditViewModel
            {
                OpeningHours = model.OpeningHours,
                Products = model.Products,
                Services = model.Services,
                Address = model.Address,
                City = model.City,
                Description = model.Description,
                Email = model.Email,
                Logo = model.Logo,
                Name = model.Name,
                Phone = model.Phone,
                PostalCode = model.PostalCode,
                Website = model.Website,
                OrganizationId = organization.OrganizationId,
                LogoId = organization.LogoId,
                ActiveInDistricts = model.ActiveInDistricts,
                ActiveOrganizationThemes = model.ActiveOrganizationThemes,
                OrganizationTypeType = model.OrganizationTypeType,
            };
        }
    }
}