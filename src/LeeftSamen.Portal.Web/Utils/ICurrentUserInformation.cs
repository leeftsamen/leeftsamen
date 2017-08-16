// <copyright file="ICurrentUserInformation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;

namespace LeeftSamen.Portal.Web.Utils
{
    using System.Data.Entity.Spatial;
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The CurrentUserInformation interface.
    /// </summary>
    public interface ICurrentUserInformation
    {
        /// <summary>
        /// Gets a value indicating whether is user in active city.
        /// </summary>
        bool IsUserInActiveCity { get; }

        /// <summary>
        /// Gets or sets the organization membership.
        /// </summary>
        OrganizationMembership OrganizationMembership { get; set; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        User User { get; }

        /// <summary>
        /// Gets the user position based on OrganizationMembership (if not null)
        /// </summary>
        DbGeography UserPosition { get; }

        /// <summary>
        /// Gets the user neighborhood radius based on OrganizationMembership (if not null)
        /// </summary>
        int UserNeighborhoodRadius { get; }

        List<Data.Models.Action> UserActions { get; }

        bool IsUserProfileValid { get; }

        void UpdateLastSeen();

        void AddDevice(string token, DeviceType type);
    }
}