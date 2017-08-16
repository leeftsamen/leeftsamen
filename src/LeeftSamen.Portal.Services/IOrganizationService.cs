// <copyright file="IOrganizationService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;

    public interface IOrganizationService
    {
        Task<OrganizationMembership> AcceptInvitationAsync(OrganizationInvitation invitation, User user);

        Task AddProductAsync(Organization organisation, string title, string introductionText, string fullText);

        Task AddServiceAsync(Organization organisation, string title, string introductionText, string fullText);

        Task<bool> DeclineInvitationAsync(OrganizationInvitation invitation, User user);

        Task DeleteProductAsync(OrganizationProduct product);

        Task DeleteServiceAsync(Data.Models.OrganizationService service);

        Task EditOrganizationAsync(
            Organization organization,
            string name,
            string description,
            OrganizationType.Types type,
            string address,
            string postalCode,
            string city,
            List<int> activeInDistricts,
            string phone,
            string email,
            string website,
            string openingHours,
            ImageDto logo,
            List<int> organizationThemes);

        Task<List<Organization>> GetAllOrganizationsActiveInNeighborhoodAsync(DbGeography position, int radius);

        Task<List<Organization>> GetAllOrganizationsActiveInNeighborhoodByThemeAsync(
            DbGeography position,
            int radius,
            int themeId);

        Task<List<Organization>> GetAllOrganizationsAsync();

        Task<List<Organization>> GetAllOrganizationsInclPendingAsync();

        Task<List<string>> GetCityNamesAsync();

        Task<Dictionary<string, List<District>>> GetDistrictsAndCitiesByCityAsync(string city);

        Task<List<District>> GetDistrictsByCityAsync(string city);

        Task<List<OrganizationInvitation>> GetInvitationsAsync(Organization organization);

        Task<OrganizationInvitation> GetInvitationByOrganizationIdAcceptTokenAsync(int? id, string code);

        Task<OrganizationInvitation> GetInvitationsByOrganizationIdAcceptTokenAsync(int? id, string code, User user);

        Task<List<OrganizationInvitation>> GetInvitationsByUserAsync(User user);

        Task<Media> GetLogoByOrganizationIdAsync(int? organizationId, int? mediaId);

        Task<List<OrganizationTheme>> GetOrganisationThemesAsync();

        Task<Organization> GetOrganizationByIdAsync(int? organizationId);

        Task<Organization> GetOrganizationByIdInclPendingAsync(int? organizationId);

        Task<OrganizationMembership> GetOrganizationMembershipByIdAsync(int? organizationId, string userId);

        Task<List<Organization>> GetOrganizationsByTypeActiveInNeighborhoodAsync(OrganizationType.Types type, DbGeography position, int radius);

        Task GiveAdminRightsAsync(OrganizationMembership membership);

        Task InviteUserAsync(
            Organization organization,
            User user,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl);

        Task InviteUsersByEmailAsync(
            Organization organization,
            IEnumerable<string> emailAddresses,
            User user,
            Func<string, string> getAcceptInvitationLinkGenerator,
            string portalUrl);

        Task<bool> IsUserAdministratorOfOrganizationAsync(Organization organization, User user);

        Task<bool> IsUserMemberOfOrganizationAsync(Organization organization, User user);

        Task RemoveMemberAsync(int organizationId, string userId);

        Task RemoveOrganizationAsync(Organization organization);

        Task RequestNewOrganizationAsync(
            string name,
            string description,
            OrganizationType.Types type,
            string address,
            string postalCode,
            string city,
            List<int> activeInDistricts,
            List<int> organizationThemes,
            string phone,
            string email,
            string website,
            string openingHours,
            ImageDto logo,
            User user);

        Task RevokeAdminRightsAsync(OrganizationMembership organizationMembership);

        Task SendInvitationReminderAsync(
            Organization organization,
            User user,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl);

        Task SendInvitationReminderByEmailAsync(
            Organization organization,
            string emailAddress,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl);

        Task<List<User>> SearchNeighborsNotInOrganizationAsync(Organization organization, User user, string q);

        Task UpdateProductAsync(OrganizationProduct product, string title, string introductionText, string fullText);

        Task UpdateServiceAsync(
            Data.Models.OrganizationService service,
            string title,
            string introductionText,
            string fullText);

        Task<Organization> ActivateOrganizationAsync(Organization organization);
    }
}