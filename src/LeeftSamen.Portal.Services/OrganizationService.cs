// <copyright file="OrganizationService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Configuration;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;
    using LeeftSamen.Portal.Services.DTO;
    using Data.Enums;
    public class OrganizationService : IOrganizationService
    {
        private readonly IApplicationDbContext databaseContext;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMailerService mailerService;

        private readonly IMediaService mediaService;

        private readonly INotificationService notificationService;

        private readonly IRandomGenerator randomGenerator;

        public OrganizationService(
            IApplicationDbContext databaseContext,
            ILinkGenerator linkGenerator,
            IMailerService mailerService,
            IMediaService mediaService,
            INotificationService notificationService,
            IRandomGenerator randomGenerator)
        {
            this.databaseContext = databaseContext;
            this.linkGenerator = linkGenerator;
            this.mailerService = mailerService;
            this.mediaService = mediaService;
            this.notificationService = notificationService;
            this.randomGenerator = randomGenerator;
        }

        private IQueryable<Organization> ApprovedOrganizations
        {
            get
            {
                return this.databaseContext.Organizations.Where(o => !o.IsRequestPending);
            }
        }

        public async Task<OrganizationMembership> AcceptInvitationAsync(OrganizationInvitation invitation, User user)
        {
            if (invitation.User != null && invitation.User.Id != user.Id)
            {
                return null;
            }

            var membership = this.databaseContext.OrganizationMemberships.Create();
            membership.Organization = invitation.Organization;
            membership.User = user;
            membership.IsAdministrator = false;
            membership.MemberSinceDateTime = DateTime.Now;

            this.databaseContext.OrganizationMemberships.Add(membership);
            this.databaseContext.OrganizationInvitations.Remove(invitation);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return membership;
        }

        public async Task<bool> DeclineInvitationAsync(OrganizationInvitation invitation, User user)
        {
            this.databaseContext.OrganizationInvitations.Remove(invitation);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task DeclineInvitationAsync(OrganizationInvitation invitation)
        {
            this.databaseContext.OrganizationInvitations.Remove(invitation);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddServiceAsync(Organization organisation, string title, string introductionText, string fullText)
        {
            var service = this.databaseContext.OrganizationServices.Create();

            service.Title = title;
            service.IntroductionText = introductionText;
            service.FullText = fullText;

            organisation.Services.Add(service);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateServiceAsync(Data.Models.OrganizationService service, string title, string introductionText, string fullText)
        {
            service.Title = title;
            service.IntroductionText = introductionText;
            service.FullText = fullText;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Organization> ActivateOrganizationAsync(Organization organization)
        {
            organization.IsRequestPending = false;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return organization;
        }

        public async Task DeleteServiceAsync(Data.Models.OrganizationService service)
        {
            this.databaseContext.OrganizationServices.Remove(service);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddProductAsync(Organization organisation, string title, string introductionText, string fullText)
        {
            var product = this.databaseContext.OrganizationProducts.Create();

            product.Organization = organisation;
            product.Title = title;
            product.IntroductionText = introductionText;
            product.FullText = fullText;

            organisation.Products.Add(product);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateProductAsync(OrganizationProduct product, string title, string introductionText, string fullText)
        {
            product.Title = title;
            product.IntroductionText = introductionText;
            product.FullText = fullText;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteProductAsync(OrganizationProduct product)
        {
            this.databaseContext.OrganizationProducts.Remove(product);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EditOrganizationAsync(
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
            List<int> organizationThemes)
        {
            organization.OpeningHours = openingHours;
            organization.Name = name;
            organization.Description = description;
            organization.Address = address;
            organization.PostalCode = postalCode;
            organization.City = city;
            organization.Phone = phone;
            organization.Email = email;

            // TODO: Normalize URL
            organization.Website = website;

            organization.OrganizationType =
                await
                this.databaseContext.OrganizationTypes.FirstOrDefaultAsync(t => t.Type == type).ConfigureAwait(false);

            if (logo != null)
            {
                if (organization.Logo != null)
                {
                    this.databaseContext.Media.Remove(organization.Logo);
                }

                organization.Logo = this.mediaService.CreateMedia(logo);
            }

            var districts =
                await
                this.databaseContext.Districts.Where(d => activeInDistricts.Contains(d.DistrictId))
                    .ToListAsync()
                    .ConfigureAwait(false);
            organization.ActiveInDistricts.Clear();
            foreach (var district in districts)
            {
                organization.ActiveInDistricts.Add(district);
            }

            var themes =
                await
                this.databaseContext.OrganizationThemes.Where(t => organizationThemes.Contains(t.ThemeId))
                    .ToListAsync()
                    .ConfigureAwait(false);
            organization.Themes.Clear();
            foreach (var theme in themes)
            {
                organization.Themes.Add(theme);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<Organization>> GetAllOrganizationsActiveInNeighborhoodAsync(DbGeography position, int radius)
        {
            return await this.GetOrganizationsActiveInNeighborhoodQuery(position, radius).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Organization>> GetAllOrganizationsActiveInNeighborhoodByThemeAsync(DbGeography position, int radius, int themeId)
        {
            return
                await
                this.GetOrganizationsActiveInNeighborhoodQuery(position, radius)
                    .Where(o => o.Themes.Any(t => t.ThemeId == themeId))
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Organization>> GetAllOrganizationsAsync()
        {
            return await this.ApprovedOrganizations.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Organization>> GetAllOrganizationsInclPendingAsync()
        {
            return await this.databaseContext.Organizations.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<string>> GetCityNamesAsync()
        {
            return
                await
                this.databaseContext.Cities.Where(c => c.CityName != string.Empty)
                    .OrderBy(c => c.CityName)
                    .Select(c => c.CityName)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<District>> GetDistrictsByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return new List<District>();
            }

            return
                await
                this.databaseContext.Districts.Where(d => d.CityName == city)
                    .OrderBy(d => d.DistrictName)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Dictionary<string, List<District>>> GetDistrictsAndCitiesByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return new Dictionary<string, List<District>>();
            }

            return
                await
                this.databaseContext.Districts.Where(d => d.CityName.StartsWith(city))
                    .GroupBy(d => d.CityName)
                    .ToDictionaryAsync(d => d.Key, d => d.ToList())
                    .ConfigureAwait(false);
        }

        public async Task<List<OrganizationInvitation>> GetInvitationsAsync(Organization organization)
        {
            return
                await
                this.databaseContext.Entry(organization)
                    .Collection(o => o.Invitations)
                    .Query()
                    .Include(i => i.User)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<OrganizationInvitation> GetInvitationByOrganizationIdAcceptTokenAsync(
            int? id,
            string acceptToken)
        {
            return
                await
                this.databaseContext.OrganizationInvitations.FirstOrDefaultAsync(
                    i => i.Organization.OrganizationId == id && i.AcceptToken == acceptToken).ConfigureAwait(false);
        }

        public async Task<OrganizationInvitation> GetInvitationsByOrganizationIdAcceptTokenAsync(
            int? id,
            string code,
            User user)
        {
            return
                await
                this.databaseContext.OrganizationInvitations.FirstOrDefaultAsync(
                    i => i.Organization.OrganizationId == id && i.AcceptToken == code && i.User.Id == user.Id)
                    .ConfigureAwait(false);
        }

        public async Task<List<OrganizationInvitation>> GetInvitationsByUserAsync(User user)
        {
            return
                await
                this.databaseContext.OrganizationInvitations.Where(c => c.User.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<OrganizationInvitation> GetUserInvitationAsync(
            Organization organization,
            User user)
        {
            return
                await
                this.databaseContext.OrganizationInvitations.FirstOrDefaultAsync(
                    i => i.Organization.OrganizationId == organization.OrganizationId && i.User.Id == user.Id).ConfigureAwait(false);
        }

        public async Task<Media> GetLogoByOrganizationIdAsync(int? organizationId, int? mediaId)
        {
            return
                await
                this.ApprovedOrganizations.Where(o => o.OrganizationId == organizationId && o.LogoId == mediaId)
                    .Select(o => o.Logo)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<OrganizationTheme>> GetOrganisationThemesAsync()
        {
            return await this.databaseContext.OrganizationThemes.ToListAsync().ConfigureAwait(false);
        }

        public async Task<Organization> GetOrganizationByIdAsync(int? organizationId)
        {
            var organization = await
                this.ApprovedOrganizations.FirstOrDefaultAsync(o => o.OrganizationId == organizationId)
                    .ConfigureAwait(false);
            if (organization.Hidden ?? false)
            {
                return null;
            }

            return organization;
        }

        public async Task<Organization> GetOrganizationByIdInclPendingAsync(int? organizationId)
        {
            return
                await
                this.databaseContext.Organizations.FirstOrDefaultAsync(o => o.OrganizationId == organizationId)
                    .ConfigureAwait(false);
        }

        public async Task<OrganizationMembership> GetOrganizationMembershipByIdAsync(int? organizationId, string userId)
        {
            return
                await
                this.databaseContext.OrganizationMemberships.FirstOrDefaultAsync(
                    m => m.Organization.OrganizationId == organizationId && m.User.Id == userId).ConfigureAwait(false);
        }

        public async Task<List<Organization>> GetOrganizationsByTypeActiveInNeighborhoodAsync(OrganizationType.Types type, DbGeography position, int radius)
        {
            return
                await
                this.GetOrganizationsActiveInNeighborhoodQuery(position, radius)
                    .Where(o => o.OrganizationType.Type == type)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task GiveAdminRightsAsync(OrganizationMembership membership)
        {
            membership.IsAdministrator = true;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task GiveAdminRightsToRequestee(Organization organization, User user)
        {
            var membership = this.databaseContext.OrganizationMemberships.Create();
            membership.IsAdministrator = true;
            membership.Organization = organization;
            membership.User = user;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InviteUserAsync(
            Organization organization,
            User user,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl)
        {
            if (!(await this.ShouldInviteUser(organization, user).ConfigureAwait(false)))
            {
                return;
            }

            var organizationInvitation =
                await this.CreateOrganizationInvitationAsync(organization, invitedBy, user).ConfigureAwait(false);

            await
                this.SendInvitationEmailAsync(organizationInvitation, acceptInvitationLinkGenerator, portalUrl)
                    .ConfigureAwait(false);
        }

        public async Task InviteUsersByEmailAsync(
            Organization organization,
            IEnumerable<string> emailAddresses,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl)
        {
            foreach (var emailAddress in emailAddresses.Where(this.mailerService.IsValidEmail))
            {
                var email = emailAddress;

                var user =
                    await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Email == email).ConfigureAwait(false);
                if (user != null && !(await this.ShouldInviteUser(organization, user).ConfigureAwait(false)))
                {
                    continue;
                }

                if (!(await this.ShouldInviteEmail(organization, email).ConfigureAwait(false)))
                {
                    continue;
                }

                var organizationInvitation =
                    await
                    this.CreateOrganizationInvitationAsync(organization, invitedBy, user, email).ConfigureAwait(false);

                await
                    this.SendInvitationEmailAsync(organizationInvitation, acceptInvitationLinkGenerator, portalUrl)
                        .ConfigureAwait(false);
            }
        }

        public async Task<bool> IsUserAdministratorOfOrganizationAsync(Organization organization, User user)
        {
            if (user.Roles.Any(r => r.RoleId == "Admin"))
            {
                return true;
            }

            return
                await
                this.databaseContext.Entry(organization)
                    .Collection(o => o.Members)
                    .Query()
                    .Where(o => o.IsAdministrator)
                    .AnyAsync(m => m.User.Id == user.Id)
                    .ConfigureAwait(false);
        }

        public async Task<bool> IsUserMemberOfOrganizationAsync(Organization organization, User user)
        {
            if (user.Roles.Any(r => r.RoleId == "Admin"))
            {
                return true;
            }

            return
                await
                this.databaseContext.Entry(organization)
                    .Collection(o => o.Members)
                    .Query()
                    .AnyAsync(m => m.User.Id == user.Id)
                    .ConfigureAwait(false);
        }

        public async Task RemoveMemberAsync(int organizationId, string userId)
        {
            var organizationMembership =
                await
                this.databaseContext.OrganizationMemberships.FirstOrDefaultAsync(
                    m => m.Organization.OrganizationId == organizationId && m.User.Id == userId).ConfigureAwait(false);

            var organizationInvitation =
                await
                this.databaseContext.OrganizationInvitations.FirstOrDefaultAsync(
                    m => m.Organization.OrganizationId == organizationId && m.User.Id == userId).ConfigureAwait(false);
            if (organizationMembership == null && organizationInvitation == null)
            {
                return;
            }

            if (organizationMembership != null)
            {
                await this.RemoveOrganizationMembershipAsync(organizationMembership).ConfigureAwait(false);
            }

            if (organizationInvitation != null)
            {
                this.databaseContext.OrganizationInvitations.Remove(organizationInvitation);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveOrganizationAsync(Organization organization)
        {
            organization.Hidden = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RequestNewOrganizationAsync(
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
            User user)
        {
            var organization = this.databaseContext.Organizations.Create();
            organization.OpeningHours = openingHours;
            organization.Name = name;
            organization.Description = description;
            organization.Address = address;
            organization.PostalCode = postalCode;
            organization.City = city;
            organization.Phone = phone;
            organization.Email = email;
            organization.Website = website;
            organization.IsRequestPending = true;
            organization.RequestedBy = user;
            organization.RequestDateTime = DateTime.Now;

            if (logo != null)
            {
                organization.Logo = this.mediaService.CreateMedia(logo);
            }

            organization.OrganizationType =
                await
                this.databaseContext.OrganizationTypes.FirstOrDefaultAsync(t => t.Type == type).ConfigureAwait(false);

            var districts = this.databaseContext.Districts.Where(d => activeInDistricts.Contains(d.DistrictId));
            foreach (var district in districts)
            {
                organization.ActiveInDistricts.Add(district);
            }

            var themes = this.databaseContext.OrganizationThemes.Where(t => organizationThemes.Contains(t.ThemeId));
            foreach (var theme in themes)
            {
                organization.Themes.Add(theme);
            }

            this.databaseContext.Organizations.Add(organization);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.GiveAdminRightsToRequestee(organization, user).ConfigureAwait(false);

            var model = new OrganizationRequestModel
                            {
                                OrganizationName = organization.Name,
                                RequestedByName = user.Name
                            };

            await
                this.mailerService.SendAsync(model, WebConfigurationManager.AppSettings["EmailPortalAdmin"])
                    .ConfigureAwait(false);
        }

        public async Task RevokeAdminRightsAsync(OrganizationMembership organizationMembership)
        {
            var numberOfAdminsLeft =
                await
                this.databaseContext.Entry(organizationMembership.Organization)
                    .Collection(o => o.Members)
                    .Query()
                    .CountAsync(
                        m =>
                        m.IsAdministrator
                        && m.OrganizationMembershipId != organizationMembership.OrganizationMembershipId)
                    .ConfigureAwait(false);
            if (numberOfAdminsLeft == 0)
            {
                return;
            }

            organizationMembership.IsAdministrator = false;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SendInvitationReminderAsync(
            Organization organization,
            User user,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl)
        {
            var organizationInvitation = await this.GetUserInvitationAsync(organization, user).ConfigureAwait(false);

            if (organizationInvitation == null)
            {
                organizationInvitation = await this.CreateOrganizationInvitationAsync(organization, invitedBy, user).ConfigureAwait(false);
            }

            await this.SendInvitationEmailAsync(organizationInvitation, acceptInvitationLinkGenerator, portalUrl).ConfigureAwait(false);
        }

        public async Task SendInvitationReminderByEmailAsync(
            Organization organization,
            string emailAddress,
            User invitedBy,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl)
        {
            var user =
                await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Email == emailAddress).ConfigureAwait(false);

            if (user == null)
            {
                return;
            }

            var organizationInvitation = await this.GetUserInvitationAsync(organization, user).ConfigureAwait(false);

            if (organizationInvitation == null)
            {
                organizationInvitation = await this.CreateOrganizationInvitationAsync(organization, invitedBy, user).ConfigureAwait(false);
            }

            await this.SendInvitationEmailAsync(organizationInvitation, acceptInvitationLinkGenerator, portalUrl).ConfigureAwait(false);
        }

        public async Task<List<User>> SearchNeighborsNotInOrganizationAsync(
            Organization organization,
            User user,
            string q)
        {
            return
                await
                this.databaseContext.Users.Where(u => u.Position.Distance(user.Position) <= user.NeighborhoodRadius)
                    .Where(u => u.Organizations.All(c => c.Organization.OrganizationId != organization.OrganizationId))
                    .Where(
                        u =>
                        u.OrganizationInvitations.All(i => i.Organization.OrganizationId != organization.OrganizationId))
                    .Where(u => u.Name.Contains(q))
                    .OrderBy(u => u.Name)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        private async Task<OrganizationInvitation> CreateOrganizationInvitationAsync(
            Organization organization,
            User invitedBy,
            User user,
            string email = null)
        {
            var organizationInvitation = this.databaseContext.OrganizationInvitations.Create();
            organizationInvitation.Organization = organization;
            organizationInvitation.InvitationDateTime = DateTime.Now;
            organizationInvitation.InvitedBy = invitedBy;
            organizationInvitation.AcceptToken = this.randomGenerator.GenerateRandomToken();
            if (user != null)
            {
                organizationInvitation.User = user;
                //await this.SendNotificationInviteAsync(user, organization).ConfigureAwait(false);
            }
            else
            {
                organizationInvitation.Email = email;
            }

            this.databaseContext.OrganizationInvitations.Add(organizationInvitation);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return organizationInvitation;
        }

        private async Task<OrganizationMembership> GetAdminFromSameOrganizationAsync(OrganizationMembership membership)
        {
            return
                await
                this.databaseContext.OrganizationMemberships.Where(
                    m => m.OrganizationMembershipId != membership.OrganizationMembershipId)
                    .Where(m => m.Organization.OrganizationId == membership.Organization.OrganizationId)
                    .FirstOrDefaultAsync(m => m.IsAdministrator)
                    .ConfigureAwait(false);
        }

        private IQueryable<Organization> GetOrganizationsActiveInNeighborhoodQuery(DbGeography position, int radius)
        {
            return
                this.ApprovedOrganizations.Where(o => (o.Hidden.HasValue && !o.Hidden.Value) || !o.Hidden.HasValue)
                    .Where(
                        o =>
                        o.ActiveInDistricts.Any(d => d.Shape.Intersects(position.Buffer(radius))));
        }

        private async Task RemoveOrganizationMembershipAsync(OrganizationMembership membership)
        {
            var admin = await this.GetAdminFromSameOrganizationAsync(membership).ConfigureAwait(false);

            foreach (var activity in
                this.databaseContext.Activities.Where(
                    a => a.OrganizationMembershipId == membership.OrganizationMembershipId))
            {
                activity.OrganizationMembership = admin;
            }

            foreach (var marketPlaceItem in
                this.databaseContext.MarketplaceItems.Where(
                    a => a.OrganizationMembershipId == membership.OrganizationMembershipId))
            {
                marketPlaceItem.OrganizationMembership = admin;
            }

            foreach (var marketPlaceItemReaction in
                this.databaseContext.MarketplaceItemReactions.Where(
                    a => a.OrganizationMembershipId == membership.OrganizationMembershipId))
            {
                marketPlaceItemReaction.OrganizationMembership = admin;
            }

            foreach (var neighborhoodMessage in
                this.databaseContext.NeighborhoodMessages.Where(
                    a => a.OrganizationMembershipId == membership.OrganizationMembershipId))
            {
                neighborhoodMessage.OrganizationMembership = admin;
            }

            this.databaseContext.OrganizationMemberships.Remove(membership);
        }

        private async Task SendInvitationEmailAsync(
            OrganizationInvitation invitation,
            Func<string, string> acceptInvitationLinkGenerator,
            string portalUrl)
        {
            var acceptInvitationUrl = acceptInvitationLinkGenerator.Invoke(invitation.AcceptToken);
            var model = new OrganizationInvitationModel
                            {
                                Subject = Subject.OrganizationInvitation,
                                AcceptInvitationUrl = acceptInvitationUrl,
                                OrganizationName = invitation.Organization.Name,
                                InvitedByName = invitation.InvitedBy.Name,
                                PortalUrl = portalUrl,
                            };

            if (invitation.User != null)
            {
                model.Name = invitation.User.Name;
                await this.mailerService.SendAsync(model, invitation.User).ConfigureAwait(false);
            }
            else
            {
                await this.mailerService.SendAsync(model, invitation.Email).ConfigureAwait(false);
            }
        }

        private async Task<bool> ShouldInviteEmail(Organization organization, string emailAddress)
        {
            var emailIsInvited =
                await
                this.databaseContext.OrganizationInvitations.AnyAsync(
                    i => i.Organization.OrganizationId == organization.OrganizationId && i.Email == emailAddress)
                    .ConfigureAwait(false);

            return !emailIsInvited;
        }

        private async Task<bool> ShouldInviteUser(Organization organization, User user)
        {
            var userEntry = this.databaseContext.Entry(user);
            var userIsMember =
                await
                userEntry.Collection(c => c.Organizations)
                    .Query()
                    .AnyAsync(c => c.Organization.OrganizationId == organization.OrganizationId)
                    .ConfigureAwait(false);
            var userIsInvited =
                await
                userEntry.Collection(c => c.OrganizationInvitations)
                    .Query()
                    .AnyAsync(c => c.Organization.OrganizationId == organization.OrganizationId)
                    .ConfigureAwait(false);

            return !userIsMember && !userIsInvited;
        }

        private async Task<bool> SendNotificationInviteAsync(User user, Organization organization)
        {
            var link = this.linkGenerator.GenerateOrganizationInviteLink(organization.OrganizationId);
            var message = string.Format(Common.InterfaceText.Notification.OrganizationInvitation, organization.Name);

            await this.notificationService.CreateNotificationForUserAsync(user, message, link, SettingName.OrganizationRequestToJoin).ConfigureAwait(false);

            return true;
        }
    }
}