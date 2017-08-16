// <copyright file="User.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        public virtual ICollection<ActivityInvitation> ActivityInvitations { get; set; }

        public virtual ICollection<CircleActivityInvitation> CircleActivityInvitations { get; set; }

        public virtual ICollection<CircleInvitation> CircleInvitations { get; set; }

        public virtual ICollection<CircleJoinRequest> CircleJoinRequests { get; set; }

        public virtual ICollection<CircleMembership> Circles { get; set; }

        public virtual string City { get; set; }

        public virtual string HouseNumber { get; set; }

        public virtual decimal Latitude { get; set; }

        public virtual decimal Longitude { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual string Gender { get; set; }

        public virtual int NeighborhoodRadius { get; set; }

        public virtual ICollection<OrganizationInvitation> OrganizationInvitations { get; set; }

        public virtual ICollection<OrganizationMembership> Organizations { get; set; }

        public virtual DbGeography Position { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual Media ProfileImage { get; set; }

        public virtual int? ProfileImageId { get; set; }

        public virtual string Street { get; set; }

        public virtual DateTime LastSeen { get; set; }

        public virtual bool IsCityPioneer { get; set; }

        public virtual bool ShowLocation { get; set; }

        public virtual bool ReceivesWeekMail { get; set; }

        public virtual bool ReceivesMarketplaceMail { get; set; }

        public virtual bool ReceivesNewMarketplaceitemMail { get; set; }

        public virtual bool ReceivesCircleMessageMail { get; set; }

        public virtual bool ReceivesCircleJobMail { get; set; }

        public virtual bool ReceivesCircleJobAssigned { get; set; }

        /// <summary>
        /// Gets or sets the device token.
        /// </summary>
        public virtual string DeviceToken { get; set; }

        /// <summary>
        /// Gets or sets the auth token.
        /// </summary>
        public virtual string AuthToken { get; set; }

        public virtual string ZuiderlingAccount { get; set; }

        public virtual bool HasZuiderlingAccount
        {
            get { return !string.IsNullOrEmpty(this.ZuiderlingAccount); }
        }

        public virtual ICollection<UserDevice> Devices { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }
}