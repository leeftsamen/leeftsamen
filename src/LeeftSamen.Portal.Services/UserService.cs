// <copyright file="UserService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>
namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Enums;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;
    using LeeftSamen.Portal.Services.DTO;

    using Microsoft.AspNet.Identity;

    public class UserService : UserManager<User>, IUserService
    {
        private readonly IApplicationDbContext databaseContext;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMailerService mailService;

        private readonly IMediaService mediaService;

        public UserService(
            IUserStore<User> store,
            IApplicationDbContext databaseContext,
            IMailerService mailService,
            IMediaService mediaService,
            ILinkGenerator linkGenerator)
            : base(store)
        {
            this.databaseContext = databaseContext;
            this.mailService = mailService;
            this.mediaService = mediaService;
            this.linkGenerator = linkGenerator;
        }

        private IQueryable<Data.Models.Action> ActiveActions
        {
            get
            {
                var now = DateTime.Now;
                return this.databaseContext.Actions.Where(
                    a =>
                    (!a.ActionStart.HasValue || a.ActionStart <= now) &&
                    (!a.ActionEnd.HasValue || a.ActionEnd >= now));
            }
        }

        public bool AllowZuiderling(User user)
        {
            if (user == null)
            {
                return false;
            }

            return this.databaseContext.ZuiderlingZipcodes.Any(z => z.ZipCode == user.PostalCode);
        }

        public async Task<bool> AllowZuiderlingAsync(User user)
        {
            if (user == null)
            {
                return false;
            }

            return await this.databaseContext.ZuiderlingZipcodes.AnyAsync(z => z.ZipCode == user.PostalCode).ConfigureAwait(false);
        }

        public async Task ChangeNameAsync(User user, string name)
        {
            user.Name = name;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeNeighborhoodAsync(
            User user,
            string postalCode,
            string houseNumber,
            string street,
            string city,
            decimal latitude,
            decimal longitude,
            int neighborhoodRadius,
            bool showLocation)
        {
            user.ShowLocation = showLocation;
            user.PostalCode = postalCode.Replace(" ", string.Empty);
            user.HouseNumber = houseNumber;
            user.Street = street;
            user.City = city;
            user.Latitude = latitude;
            user.Longitude = longitude;
            user.Position = CreatePoint(latitude, longitude);
            user.NeighborhoodRadius = neighborhoodRadius;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeUsernameAsync(User user, string username)
        {
            user.UserName = username;
            user.Email = username;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeZuiderlingAccountAsync(User user, string zuiderlingAccount)
        {
            user.ZuiderlingAccount = zuiderlingAccount;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeEmailSettingsAsync(User user)
        {
            this.databaseContext.Entry(user).State = EntityState.Modified;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<User> CreateUserAsync(
            string name,
            string email,
            string password,
            string city,
            string houseNumber,
            string postalCode,
            string street,
            decimal latitude,
            decimal longitude,
            int neighborhoodRadius,
            string portalUrl)
        {
            var user = this.databaseContext.Users.Create();
            user.UserName = email;
            user.Email = email;
            user.Name = name;
            user.City = city;
            user.HouseNumber = houseNumber;
            user.PostalCode = postalCode.Replace(" ", string.Empty);
            user.Street = street;
            user.Latitude = latitude;
            user.Longitude = longitude;
            user.NeighborhoodRadius = neighborhoodRadius;
            user.Position = CreatePoint(latitude, longitude);
            user.ReceivesCircleJobAssigned = true;
            user.ReceivesCircleJobMail = true;
            user.ReceivesCircleMessageMail = true;
            user.ReceivesMarketplaceMail = true;
            user.ReceivesWeekMail = true;
            user.ShowLocation = true;

            var result = await this.CreateAsync(user, password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return null;
            }

            var code = await this.GenerateEmailConfirmationTokenAsync(user.Id).ConfigureAwait(false);
            var confirmEmailUrl = this.linkGenerator.GenerateAccountConfirmEmailLink(user.Id, code);
            var emailModel = new WelcomeModel
            {
                Subject = Subject.Welcome,
                ConfirmEmailUrl = confirmEmailUrl,
                Name = user.Name,
                PortalUrl = portalUrl
            };

            await this.mailService.SendAsync(emailModel, user).ConfigureAwait(false);
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.databaseContext.Users.ToListAsync().ConfigureAwait(false);
        }

        public async Task<Media> GetProfileImageByUserIdAsync(string userId, int? mediaId)
        {
            return
                await
                this.databaseContext.Users.Where(u => u.Id == userId && u.ProfileImageId == mediaId)
                    .Select(u => u.ProfileImage)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
        }

        public async Task<OrganizationMembership> GetUserOrganizationMembershipByIdAsync(
            User user,
            int? organizationMembershipId)
        {
            return
                await
                this.databaseContext.Entry(user)
                    .Collection(u => u.Organizations)
                    .Query()
                    .FirstOrDefaultAsync(m => m.OrganizationMembershipId == organizationMembershipId)
                    .ConfigureAwait(false);
        }

        public async Task<List<Data.Models.Action>> GetUserActionsByIdAsync(User user)
        {
            return
                await
                this.ActiveActionsForUser(user)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<User>> GetUsersInRangeAsync(decimal latitude, decimal longitude, int radiusInMeters)
        {
            var point = CreatePoint(latitude, longitude);

            return
                await
                this.databaseContext.Users.Where(u => point.Distance(u.Position) <= radiusInMeters)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<bool> IsUserInActiveCity(User user)
        {
            var city =
                await
                this.databaseContext.Cities.FirstOrDefaultAsync(c => c.CityName == user.City).ConfigureAwait(false);

            return city != null && city.Enabled;
        }

        public async Task MakeUserPioneerAsync(User user)
        {
            user.IsCityPioneer = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            var message = "Er is een nieuwe aanmelding van een pionier: " + user.Email + "\n\n"
                + user.Name + "\n"
                + user.Street + " " + user.HouseNumber + "\n"
                + user.PostalCode + " " + user.City;

            await this.mailService.SendAsync("info@leeftsamen.nl", "Nieuwe pionier", message).ConfigureAwait(false);
        }

        public async Task RemoveUserAsync(User user, ICircleService circleService, string portalUrl)
        {
            await this.RemoveUserCirclesAsync(user, circleService, portalUrl).ConfigureAwait(false);

            var userHelpIcons = this.databaseContext.HelpIconUsers.Where(h => h.User.Id == user.Id).ToList();
            foreach (var icon in userHelpIcons)
            {
                this.databaseContext.HelpIconUsers.Remove(icon);
            }

            var organizationInvitations = this.databaseContext.OrganizationInvitations.Where(o => o.InvitedBy.Id == user.Id || o.User.Id == user.Id).ToList();
            foreach (var invite in organizationInvitations)
            {
                this.databaseContext.OrganizationInvitations.Remove(invite);
            }

            var marketplaceItemReactions = this.databaseContext.MarketplaceItemReactions.Where(r => r.Creator.Id == user.Id).ToList();
            foreach (var reaction in marketplaceItemReactions)
            {
                var childReactions = this.databaseContext.MarketplaceItemReactions.Where(r => r.ParentId == reaction.ReactionId);
                this.databaseContext.MarketplaceItemReactions.Remove(reaction);
                foreach (var childReaction in childReactions)
                {
                    this.databaseContext.MarketplaceItemReactions.Remove(childReaction);
                }
            }

            var marketplaceItems = this.databaseContext.MarketplaceItems.Where(r => r.Owner.Id == user.Id).ToList();
            foreach (var marketplaceItem in marketplaceItems)
            {
                if (marketplaceItem.Image1 != null)
                {
                    this.databaseContext.Media.Remove(marketplaceItem.Image1);
                }

                if (marketplaceItem.Image2 != null)
                {
                    this.databaseContext.Media.Remove(marketplaceItem.Image2);
                }

                if (marketplaceItem.Image3 != null)
                {
                    this.databaseContext.Media.Remove(marketplaceItem.Image3);
                }

                if (marketplaceItem.Image4 != null)
                {
                    this.databaseContext.Media.Remove(marketplaceItem.Image4);
                }

                if (marketplaceItem.Image5 != null)
                {
                    this.databaseContext.Media.Remove(marketplaceItem.Image5);
                }

                this.databaseContext.MarketplaceItems.Remove(marketplaceItem);
            }

            await this.RemoveUserDevicesAsync(user).ConfigureAwait(false);
            
            if (user.ProfileImage != null)
            {
                this.databaseContext.Media.Remove(user.ProfileImage);
            }

            this.databaseContext.Users.Remove(user);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SendResetPasswordLinkAsync(User user, string portalUrl)
        {
            var code = await this.GeneratePasswordResetTokenAsync(user.Id).ConfigureAwait(false);
            var callbackUrl = this.linkGenerator.GenerateAccountPasswordResetLink(user.Id, code);

            var model = new ResetPasswordModel
            {
                Subject = Subject.ResetPassword,
                ResetPasswordUrl = callbackUrl,
                PortalUrl = portalUrl,
                Name = user.Name
            };

            await this.mailService.SendAsync(model, user).ConfigureAwait(false);
        }

        public async Task RegisterUserDevice(User user, string token, DeviceType type)
        {
            var existing = this.databaseContext.UserDevices.FirstOrDefault(d => d.User.Id == user.Id && d.Token == token && d.DeviceType == type);

            if (existing != null)
            {
                existing.LastUseDate = DateTime.Now;
            }
            else
            {
                var device = new UserDevice
                {
                    LastUseDate = DateTime.Now,
                    User = user,
                    Token = token,
                    DeviceType = type
                };

                this.databaseContext.UserDevices.Add(device);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<UserDevice>> GetActiveUserDevices(User user)
        {
            var date = DateTime.Now.AddDays(-14);
            return
                await
                this.databaseContext.UserDevices.Where(ud => ud.User.Id == user.Id && ud.LastUseDate > date)
                    .OrderByDescending(ud => ud.LastUseDate)
                    .Take(3)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<UserDevice>> GetAllUserDevices(User user)
        {
            return await this.databaseContext.UserDevices.Where(ud => ud.User == user).ToListAsync().ConfigureAwait(false);
        }

        public async Task CustomNotifyByEmailAsync(User user, BasicMailModel model)
        {
            await this.mailService.SendAsync(model, user).ConfigureAwait(false);
        }

        public async Task CustomNotifyByEmailAsync(string emailAddress, BasicMailModel model)
        {
            await this.mailService.SendAsync(model, emailAddress).ConfigureAwait(false);
        }

        public async Task SendInvitation(User invitedBy, List<string> emailAdresses, string message, string portalUrl)
        {
            var mail = new InviteUserModel
            {
                Message = message,
                Subject = string.Format(Subject.InviteUser, invitedBy.Name),
                PortalUrl = portalUrl
            };
            foreach (var email in emailAdresses)
            {
                var user =
                    this.databaseContext.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    await this.mailService.SendAsync(mail, email).ConfigureAwait(false);
                }
            }
        }

        public async Task SetProfileImageAsync(User user, ImageDto image)
        {
            if (user.ProfileImage != null)
            {
                this.databaseContext.Media.Remove(user.ProfileImage);
            }

            user.ProfileImage = this.mediaService.CreateMedia(image);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<User> UpdateUserLastSeenAsync(User user)
        {
            user.LastSeen = DateTime.Now;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return user;
        }

        public async Task<bool> UserCanBecomePioneerAsync(User user)
        {
            if (user.IsCityPioneer)
            {
                // is already pioneer
                return false;
            }

            var city =
                await
                this.databaseContext.Cities.FirstOrDefaultAsync(c => c.CityName == user.City).ConfigureAwait(false);

            if (city == null || city.Enabled)
            {
                // city doesn't need a pioneer
                return false;
            }

            var cityPioneers =
                await this.databaseContext.Users.CountAsync(u => u.City == city.CityName && u.IsCityPioneer).ConfigureAwait(false);
            return cityPioneers <= 3;
        }

        public bool UserExistsWithAcceptToken(string token)
        {
            var exists = this.databaseContext.ActivityInvitations.Any(a => a.AcceptToken == token && a.User != null);
            if (!exists)
            {
                exists = this.databaseContext.CircleInvitations.Any(c => c.AcceptToken == token && c.User != null);
            }

            return exists;
        }

        private static DbGeography CreatePoint(decimal latitude, decimal longitude)
        {
            // 4326 is the coordinateSystemId for the Geographic coordinate system, aka Latitude/longitude
            return
                DbGeography.PointFromText(
                    string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", longitude, latitude),
                    4326);
        }

        private IQueryable<Data.Models.Action> ActiveActionsForUser(User user)
        {
            return this.ActiveActions.Where(
                a =>
                    a.Zipcodes.Any(z => z.PostalCode == user.PostalCode));
        }

        private async Task RemoveUserCirclesAsync(User user, ICircleService circleService, string portalUrl)
        {
            var circles = await this.databaseContext.Circles.Where(c => c.Creator.Id == user.Id).ToListAsync().ConfigureAwait(false);
            foreach (var circle in circles)
            {
                circle.Creator = null;
            }

            await this.RemoveUserActivitiesAsync(user).ConfigureAwait(false);
            await this.RemoveUserCircleActivitiesAsync(user).ConfigureAwait(false);
            await this.RemoveUserCircleEmailAsync(user).ConfigureAwait(false);

            var circleInvitations =
                await
                this.databaseContext.CircleInvitations.Where(c => c.InvitedBy.Id == user.Id || c.User.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var invite in circleInvitations)
            {
                this.databaseContext.CircleInvitations.Remove(invite);
            }

            var circleJoinRequests =
                await
                this.databaseContext.CircleJoinRequests.Where(c => c.User.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circleJoinRequest in circleJoinRequests)
            {
                this.databaseContext.CircleJoinRequests.Remove(circleJoinRequest);
            }

            var jobs = this.databaseContext.Jobs.Where(j => j.Creator.Id == user.Id).ToList();
            foreach (var job in jobs)
            {
                this.databaseContext.Jobs.Remove(job);
            }

            jobs = this.databaseContext.Jobs.Where(j => j.Assignee.Id == user.Id).ToList();
            foreach (var job in jobs)
            {
                await circleService.UnassignUserToJobAsync(job, user, portalUrl);
            }


            var photos = this.databaseContext.CirclePhotos.Where(p => p.UploadedBy.Id == user.Id).Include(p => p.Photo).ToList();
            foreach (var photo in photos)
            {
                this.databaseContext.Media.Remove(photo.Photo);
                this.databaseContext.CirclePhotos.Remove(photo);
            }

            var albums = this.databaseContext.CirclePhotoAlbums.Where(a => a.CreatedBy.Id == user.Id).ToList();
            foreach (var album in albums)
            {
                this.databaseContext.CirclePhotoAlbums.Remove(album);
            }

            var messageReactions = this.databaseContext.CircleMessageReactions.Where(m => m.Creator.Id == user.Id).ToList();
            foreach (var reaction in messageReactions)
            {
                this.databaseContext.CircleMessageReactions.Remove(reaction);
            }

            var messages = this.databaseContext.CircleMessages.Where(m => m.Creator.Id == user.Id).ToList();
            foreach (var message in messages)
            {
                var childReactions = this.databaseContext.CircleMessageReactions.Where(r => r.Message.MessageId == message.MessageId).ToList();
                foreach (var childReaction in childReactions)
                {
                    this.databaseContext.CircleMessageReactions.Remove(childReaction);
                }
                this.databaseContext.CircleMessages.Remove(message);
            }

            var circlesToRemove = this.databaseContext.Circles.Where(c => c.Members.Count == 1 && c.Members.FirstOrDefault().User.Id == user.Id).ToList();
            foreach (var circle in circlesToRemove)
            {
                await circleService.RemoveCircleAsync(circle);
            }
        }

        private async Task RemoveUserActivitiesAsync(User user)
        {
            var activityAttendances =
                await
                this.databaseContext.ActivityAttendances.Where(
                    aa => aa.User.Id == user.Id || aa.Activity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityAttendance in activityAttendances)
            {
                this.databaseContext.ActivityAttendances.Remove(activityAttendance);
            }

            var activityIntervals =
                await
                this.databaseContext.ActivityIntervals.Where(ai => ai.Activity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityInterval in activityIntervals)
            {
                this.databaseContext.ActivityIntervals.Remove(activityInterval);
            }

            var activityInvitations =
                await
                this.databaseContext.ActivityInvitations.Where(
                    ai => ai.User.Id == user.Id || ai.InvitedBy.Id == user.Id || ai.Activity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityInvitation in activityInvitations)
            {
                this.databaseContext.ActivityInvitations.Remove(activityInvitation);
            }

            var activityReactions =
                await
                this.databaseContext.ActivityReactions.Where(
                    ar => ar.Creator.Id == user.Id || ar.Activity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityReaction in activityReactions)
            {
                this.databaseContext.ActivityReactions.Remove(activityReaction);
            }

            var activities =
                await
                this.databaseContext.Activities.Where(a => a.Creator.Id == user.Id).ToListAsync().ConfigureAwait(false);
            foreach (var activity in activities)
            {
                this.databaseContext.Activities.Remove(activity);
            }
        }

        private async Task RemoveUserCircleActivitiesAsync(User user)
        {
            var activityAttendances =
                await
                this.databaseContext.CircleActivityAttendances.Where(
                    aa => aa.User.Id == user.Id || aa.CircleActivity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityAttendance in activityAttendances)
            {
                this.databaseContext.CircleActivityAttendances.Remove(activityAttendance);
            }

            var activityInvitations =
                await
                this.databaseContext.CircleActivityInvitations.Where(
                    ai => ai.User.Id == user.Id || ai.InvitedBy.Id == user.Id || ai.CircleActivity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityInvitation in activityInvitations)
            {
                this.databaseContext.CircleActivityInvitations.Remove(activityInvitation);
            }

            var activityReactions =
                await
                this.databaseContext.CircleActivityReactions.Where(
                    ar => ar.Creator.Id == user.Id || ar.CircleActivity.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityReaction in activityReactions)
            {
                this.databaseContext.CircleActivityReactions.Remove(activityReaction);
            }

            var activities =
                await
                this.databaseContext.CircleActivities.Where(a => a.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activity in activities)
            {
                this.databaseContext.CircleActivities.Remove(activity);
            }
        }

        private async Task RemoveUserCircleEmailAsync(User user)
        {
            var messageReceivers =
                await
                this.databaseContext.CircleEmailMessageReceivers.Where(
                    r => r.Receiver.Id == user.Id || r.EmailGroup.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circleEmailMessageReceiver in messageReceivers)
            {
                this.databaseContext.CircleEmailMessageReceivers.Remove(circleEmailMessageReceiver);
            }

            var emailMessages =
                await
                this.databaseContext.CircleEmailMessages.Where(
                    m => m.Creator.Id == user.Id || m.Group.Creator.Id == user.Id).ToListAsync().ConfigureAwait(false);
            foreach (var circleEmailMessage in emailMessages)
            {
                this.databaseContext.CircleEmailMessages.Remove(circleEmailMessage);
            }

            var emailGroups =
                await
                this.databaseContext.CircleEmailGroups.Where(g => g.Creator.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circleEmailGroup in emailGroups)
            {
                this.databaseContext.CircleEmailGroups.Remove(circleEmailGroup);
            }
        }

        private async Task RemoveUserDevicesAsync(User user)
        {
            var userDevices =
                await
                this.databaseContext.UserDevices.Where(ud => ud.User.Id == user.Id).ToListAsync().ConfigureAwait(false);
            foreach (var userDevice in userDevices)
            {
                this.databaseContext.UserDevices.Remove(userDevice);
            }
        }
    }
}
