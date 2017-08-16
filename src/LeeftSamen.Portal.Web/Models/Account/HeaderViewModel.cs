// <copyright file="HeaderViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.Collections.Generic;
    using System.Linq;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The header view model.
    /// </summary>
    public class HeaderViewModel
    {
        /// <summary>
        /// The organizations.
        /// </summary>
        private List<OrganizationViewModel> organizations;

        /// <summary>
        /// Gets or sets the current organization.
        /// </summary>
        public OrganizationViewModel CurrentOrganization { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        public List<OrganizationViewModel> Organizations
        {
            get
            {
                if (this.CurrentOrganization != null)
                {
                    return
                        this.organizations.Where(
                            o => o.OrganizationMembershipId != this.CurrentOrganization.OrganizationMembershipId)
                            .ToList();
                }

                return this.organizations;
            }

            set
            {
                this.organizations = value;
            }
        }

        /// <summary>
        /// Gets or sets the profile image id.
        /// </summary>
        public int? ProfileImageId { get; set; }

        /// <summary>
        /// Gets or sets the unread notifications.
        /// </summary>
        public List<Notification> LatestNotifications { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        public bool AllowZuiderling { get; set; }

        public bool HasZuiderling { get; set; }

        public decimal? ZuiderlingAmount { get; set; }

        /// <summary>
        /// The organization view model.
        /// </summary>
        public class OrganizationViewModel
        {
            /// <summary>
            /// Gets or sets the organization membership id.
            /// </summary>
            public int OrganizationMembershipId { get; set; }

            /// <summary>
            /// Gets or sets the organization name.
            /// </summary>
            public string OrganizationName { get; set; }
        }
    }
}