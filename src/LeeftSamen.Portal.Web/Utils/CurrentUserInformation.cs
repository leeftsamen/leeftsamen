// <copyright file="CurrentUserInformation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Collections.Generic;
using LeeftSamen.Portal.Data.Enums;

namespace LeeftSamen.Portal.Web.Utils
{
    using System;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// The current user information.
    /// </summary>
    internal class CurrentUserInformation : ICurrentUserInformation
    {
        /// <summary>
        /// The session key organization membership id.
        /// </summary>
        private const string SessionKeyOrganizationMembershipId = "OrganizationMembershipId";

        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        private bool lastSeenUpdated;

        /// <summary>
        /// The is user in active city.
        /// </summary>
        private bool? isUserInActiveCity;

        /// <summary>
        /// The organization membership.
        /// </summary>
        private OrganizationMembership organizationMembership;

        /// <summary>
        /// The organization membership loaded.
        /// </summary>
        private bool organizationMembershipLoaded;

        /// <summary>
        /// The user.
        /// </summary>
        private User user;

        private List<Data.Models.Action> userActions;

        /// <summary>
        /// The user profile is valid
        /// </summary>
        private bool? isUserProfileValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserInformation"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user service.
        /// </param>
        public CurrentUserInformation(IUserService userService)
        {
            // TODO: Don't depend on HttpContext global, instead inject IPrincipal and Session objects.
            this.userService = userService;
        }

        /// <summary>
        /// Gets a value indicating whether is user in active city.
        /// </summary>
        public bool IsUserInActiveCity
        {
            get
            {
                if (this.isUserInActiveCity.HasValue)
                {
                    return this.isUserInActiveCity.Value;
                }

                if (this.User == null)
                {
                    this.isUserInActiveCity = false;
                }
                else
                {
                    this.isUserInActiveCity = this.userService.IsUserInActiveCity(this.User).Result;
                }

                return this.isUserInActiveCity.Value;
            }
        }

        /// <summary>
        /// Gets or sets the organization membership.
        /// </summary>
        public OrganizationMembership OrganizationMembership
        {
            get
            {
                if (!this.organizationMembershipLoaded)
                {
                    int? organizationMembershipId = null;
                    if (HttpContext.Current.Session != null)
                    {
                        organizationMembershipId =
                            HttpContext.Current.Session[SessionKeyOrganizationMembershipId] as int?;
                    }

                    this.organizationMembership =
                        this.userService.GetUserOrganizationMembershipByIdAsync(this.User, organizationMembershipId)
                            .Result;
                    this.organizationMembershipLoaded = true;
                }

                return this.organizationMembership;
            }

            set
            {
                this.organizationMembership = value;
                this.organizationMembershipLoaded = true;
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session.Remove(SessionKeyOrganizationMembershipId);
                    if (this.organizationMembership != null)
                    {
                        HttpContext.Current.Session[SessionKeyOrganizationMembershipId] =
                            this.organizationMembership.OrganizationMembershipId;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        public User User
        {
            get
            {
                if (HttpContext.Current.User == null)
                {
                    return null;
                }

                return this.user
                       ?? (this.user =
                           this.userService.GetUserByIdAsync(HttpContext.Current.User.Identity.GetUserId()).Result);
            }
        }

        public DbGeography UserPosition
        {
            get
            {
                return this.OrganizationMembership == null
                           ? this.User.Position
                           : this.OrganizationMembership.Organization.ActiveInDistricts
                                 .Aggregate<District, DbGeography>(
                                     null,
                                     (current, district) =>
                                     current == null ? district.Shape : current.Union(district.Shape));
            }
        }

        public int UserNeighborhoodRadius
        {
            get
            {
                return this.OrganizationMembership == null
                           ? this.User.NeighborhoodRadius
                           : 0;
            }
        }

        public List<Data.Models.Action> UserActions
        {
            get
            {
                if (this.userActions == null)
                {
                    this.userActions = this.userService.GetUserActionsByIdAsync(this.User).Result;
                }

                return this.userActions;
            }
        }

        public bool IsUserProfileValid
        {
            get
            {
                if (this.isUserProfileValid.HasValue)
                {
                    return this.isUserProfileValid.Value;
                }

                if (this.User == null)
                {
                    this.isUserProfileValid = false;
                }
                else
                {
                    this.isUserProfileValid = !String.IsNullOrWhiteSpace(this.User.PostalCode)
                        && !String.IsNullOrWhiteSpace(this.User.HouseNumber);
                }

                return this.isUserProfileValid.Value;
            }
        }

        public void UpdateLastSeen()
        {
            if (this.lastSeenUpdated || this.User == null)
            {
                return;
            }

            // update LastSeen only every 5 minutes because update queries are slow
            if (DateTime.Now - this.user.LastSeen > TimeSpan.FromMinutes(5))
            {
                this.userService.UpdateUserLastSeenAsync(this.user).Wait();
            }

            this.lastSeenUpdated = true;
        }

        public void AddDevice(string token, DeviceType type)
        {
            this.userService.RegisterUserDevice(this.user, token, type).Wait();
        }
    }
}