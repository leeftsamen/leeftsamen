// <copyright file="DetailViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The detail view model.
    /// </summary>
    public class DetailViewModel
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; }

        public string OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is organization administrator.
        /// </summary>
        public bool CurrentUserIsOrganizationAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets a value indicating whether has email.
        /// </summary>
        public bool HasEmail
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Email);
            }
        }

        /// <summary>
        /// Gets a value indicating whether has phone.
        /// </summary>
        public bool HasPhone
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Phone);
            }
        }

        /// <summary>
        /// Gets a value indicating whether has website.
        /// </summary>
        public bool HasWebsite
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Website);
            }
        }

        /// <summary>
        /// Gets a value indicating whether has website.
        /// </summary>
        public bool HasOpeningHours
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.OpeningHours);
            }
        }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public List<NeighborhoodMessage> Messages { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets the website display text.
        /// </summary>
        public string WebsiteDisplayText
        {
            get
            {
                return this.HasWebsite ? Regex.Replace(this.Website, @"http[s]?://", string.Empty).Replace("//", string.Empty) : null;
            }
        }
    }
}