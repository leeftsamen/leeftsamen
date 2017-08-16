// <copyright file="AutomapperConfig.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using LeeftSamen.Portal.Web.Utils;
using RazorEngine.Compilation.ImpromptuInterface.Dynamic;

namespace LeeftSamen.Portal.Web
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoMapper;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;
    using LeeftSamen.Portal.Web.Models.Account;
    using LeeftSamen.Portal.Web.Models.Activities;
    using LeeftSamen.Portal.Web.Models.Circles;
    using LeeftSamen.Portal.Web.Models.Marketplace;
    using LeeftSamen.Portal.Web.Models.NeighborhoodMessages;
    using LeeftSamen.Portal.Web.Models.Notifications;
    using LeeftSamen.Portal.Web.Models.Organizations;

    using DetailHeaderViewModel = LeeftSamen.Portal.Web.Models.Circles.DetailHeaderViewModel;
    using DetailViewModel = LeeftSamen.Portal.Web.Models.Circles.DetailViewModel;
    using IndexViewModel = LeeftSamen.Portal.Web.Models.Circles.IndexViewModel;
    using MembersViewModel = LeeftSamen.Portal.Web.Models.Circles.MembersViewModel;

    /// <summary>
    /// The auto mapper config.
    /// </summary>
    public class AutomapperConfig
    {
        /// <summary>
        /// The configure mappings.
        /// </summary>
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<Circle, DetailViewModel>()
                .ForMember(m => m.TimeLineItems, options => options.Ignore())
                .ForMember(m => m.IsCurrentUserMember, options => options.Ignore());
            Mapper.CreateMap<Circle, IndexViewModel.CircleViewModel>()
                .ForMember(m => m.MemberCount, options => options.Ignore())
                .ForMember(m => m.HasRequestedToJoin, options => options.Ignore())
                .ForMember(m => m.IsCurrentUserMember, options => options.Ignore())
                .ForMember(m => m.IsCurrentUserAdmin, options => options.Ignore())
                .ForMember(m => m.HelpIcons, options => options.Ignore())
                .ForMember(m => m.IsInvitedToJoin, options => options.Ignore());

            Mapper.CreateMap<Circle, DetailHeaderViewModel>()
                .ForMember(h => h.CurrentUserCanLeaveCircle, options => options.Ignore())
                .ForMember(h => h.CurrentUserIsMember, options => options.Ignore())
                .ForMember(m => m.CurrentUserHasRequestedToJoin, options => options.Ignore())
                .ForMember(h => h.MenuItems, options => options.Ignore())
                .ForMember(h => h.CurrentUserIsCircleAdministrator, options => options.Ignore())
                .ForMember(h => h.HelpIcons, options => options.Ignore())
                .ForMember(h => h.CurrentUserCanOnlyView, options => options.Ignore())
                .ForMember(h => h.CurrentUserIsInvitedToJoin, options => options.Ignore())
                .ForMember(h => h.InvitationToken, options => options.Ignore())
                .ForMember(h => h.ReceiveEmails, options => options.Ignore());

            Mapper.CreateMap<CircleJoinRequest, JoinRequestsViewModel.JoinRequestViewModel>();

            Mapper.CreateMap<Job, Models.Jobs.IndexViewModel.JobViewModel>()
                .ForMember(m => m.HasDueDateTime, options => options.MapFrom(u => u.HasDueTime));

            Mapper.CreateMap<Job, Models.Jobs.EditPostModel>()
                .ForMember(m => m.CompletionDateTimeHour, options => options.MapFrom(u => u.CompletionDateTime.Value.Hour))
                .ForMember(m => m.CompletionDateTimeMinute, options => options.MapFrom(u => u.CompletionDateTime.Value.Minute))
                .ForMember(m => m.HasCompletionDateTimeHour, options => options.MapFrom(u => u.CompletionDateTime.HasValue))
                .ForMember(m => m.HasCompletionDateTimeMinute, options => options.MapFrom(u => u.CompletionDateTime.HasValue))
                .ForMember(m => m.HasNoDueDate, options => options.MapFrom(u => !u.HasDueTime))
                .ForMember(m => m.HasDueDateTimeHour, options => options.MapFrom(u => u.DueDateTime.Hour != null))
                .ForMember(m => m.HasDueDateTimeMinute, options => options.MapFrom(u => u.DueDateTime.Minute != null))
                .ForMember(m => m.DueDateTimeEndHour, options => options.MapFrom(u => u.DueDateTimeEnd != null ? (int?)u.DueDateTimeEnd.Value.Hour : null))
                .ForMember(m => m.DueDateTimeEndMinute, options => options.MapFrom(u => u.DueDateTimeEnd != null ? (int?)u.DueDateTimeEnd.Value.Minute : null))
                .ForMember(m => m.HasDueDateTimeEndHour, options => options.MapFrom(u => true))//u.DueDateTime != null))
                .ForMember(m => m.HasDueDateTimeEndMinute, options => options.MapFrom(u => true))// u.DueDateTime != null))
                .ForMember(m => m.SelectedMembershipIds, options => options.MapFrom(u => u.VisibleToMembers))
                .ForMember(m => m.CircleId, options => options.MapFrom(u => u.Circle.CircleId))
                .ForMember(m => m.Repeat, options => options.Ignore())
                .ForMember(m => m.Weekdays, options => options.Ignore());

            Mapper.CreateMap<User, Models.Circles.SearchUserViewModel>()
                .ForMember(m => m.UserId, options => options.MapFrom(u => u.Id))
                .ForMember(m => m.ProfileImageUrl, options => options.Ignore())
                .ForMember(m => m.HasProfileImage, options => options.MapFrom(u => u.ProfileImageId.HasValue));

            Mapper.CreateMap<User, Models.Squares.SearchUserViewModel>()
                .ForMember(m => m.UserId, options => options.MapFrom(u => u.Id))
                .ForMember(m => m.ProfileImageUrl, options => options.Ignore())
                .ForMember(m => m.HasProfileImage, options => options.MapFrom(u => u.ProfileImageId.HasValue));

            Mapper.CreateMap<User, HeaderViewModel>()
                .ForMember(m => m.CurrentOrganization, options => options.Ignore())
                .ForMember(m => m.HelpIcons, options => options.Ignore())
                .ForMember(m => m.LatestNotifications, options => options.Ignore())
                .ForMember(m => m.AllowZuiderling, options => options.Ignore())
                .ForMember(m => m.HasZuiderling, options => options.Ignore())
                .ForMember(m => m.ZuiderlingAmount, options => options.Ignore())
                .ForMember(m => m.Organizations, options => options.MapFrom(src => src.Organizations.Where(o => o.Organization.Hidden != true)));
            Mapper.CreateMap<OrganizationMembership, HeaderViewModel.OrganizationViewModel>();
            Mapper.CreateMap<User, ChangeProfileImageViewModel>()
                .ForMember(m => m.ProfileImage, options => options.Ignore());

            Mapper.CreateMap<CircleMembership, SearchMemberViewModel>()
                .ForMember(m => m.ProfileImageUrl, options => options.Ignore())
                .ForMember(m => m.HasProfileImage, options => options.MapFrom(u => u.User.ProfileImageId.HasValue));

            Mapper.CreateMap<CircleMembership, MembersViewModel.MemberViewModel>()
                .ForMember(m => m.Email, options => options.Ignore())
                .ForMember(m => m.HasBeenInvited, options => options.UseValue(false))
                .ForMember(m => m.InvitationIsExpired, options => options.UseValue(false))
                .ForMember(m => m.ProfileDescription, options => options.MapFrom(u => u.Profile));
            Mapper.CreateMap<CircleInvitation, MembersViewModel.MemberViewModel>()
                .ForMember(m => m.IsAdministrator, options => options.UseValue(false))
                .ForMember(m => m.HasBeenInvited, options => options.UseValue(true))
                .ForMember(m => m.InvitationIsExpired, options => options.ResolveUsing(m => m.ExpireDate < DateTime.Now))
                .ForMember(m => m.ProfileDescription, options => options.Ignore());

            Mapper.CreateMap<User, Models.Squares.MembersViewModel.MemberViewModel>();

            Mapper.CreateMap<MarketplaceItem, ItemViewModel>()
                .ForMember(m => m.Image1, options => options.Ignore())
                .ForMember(m => m.Image2, options => options.Ignore())
                .ForMember(m => m.Image3, options => options.Ignore())
                .ForMember(m => m.Image4, options => options.Ignore())
                .ForMember(m => m.Image5, options => options.Ignore())
                .ForMember(i => i.Categories, options => options.Ignore())
                .ForMember(i => i.CategoriesList, options => options.Ignore())
                .ForMember(i => i.CirleSelectListItems, options => options.Ignore())
                .ForMember(i => i.ShowInCircle, options => options.Ignore())
                .ForMember(i => i.AllowZuiderling, options => options.Ignore())
                .ForMember(i => i.CircleId, options => options.Ignore())
                .ForMember(i => i.SocialVersion, options => options.Ignore());

            Mapper.CreateMap<MarketplaceItemPrivateReactionsDto, Models.Marketplace.DetailViewModel>()
                .ForMember(i => i.CurrentUserCanVIew, options => options.Ignore())
                .ForMember(i => i.CurrentUserCanEdit, options => options.Ignore())
                .ForMember(i => i.CurrentUserCanPlaceBaseReaction, options => options.Ignore())
                .ForMember(i => i.NewReaction, options => options.Ignore())
                .ForMember(i => i.NewReactionParentId, options => options.Ignore())
                .ForMember(m => m.AllowSharing, options => options.Ignore())
                .ForMember(m => m.ShownInCircle, options => options.Ignore())
                .ForMember(i => i.UserTransactionStatus, options => options.Ignore())
                .ForMember(i => i.AllowZuiderling, options => options.Ignore());

            Mapper.CreateMap<Organization, Models.Organizations.IndexViewModel.OrganizationViewModel>()
                .ForMember(m => m.OrganizationTypeName, options => options.MapFrom(o => o.OrganizationType.Name));
            //Mapper.CreateMap<Organization, Models.Action.IndexVoteModel.OrganizationViewModel>()
            //    .ForMember(m => m.OrganizationTypeName, options => options.MapFrom(o => o.OrganizationType.Name))
            //    .ForMember(m => m.HasVoted, options => options.Ignore());
            Mapper.CreateMap<Organization, Models.Organizations.DetailHeaderViewModel>()
                .ForMember(m => m.MenuItems, options => options.Ignore())
                .ForMember(m => m.CurrentUserIsOrganizationAdministrator, options => options.Ignore())
                .ForMember(m => m.CurrentUserIsMember, options => options.Ignore());

            Mapper.CreateMap<OrganizationInvitation, Models.Organizations.MembersViewModel.MemberViewModel>()
                .ForMember(m => m.IsAdministrator, options => options.UseValue(false))
                .ForMember(m => m.HasBeenInvited, options => options.UseValue(true));

            Mapper.CreateMap<Square, Models.Squares.DetailHeaderViewModel>()
                .ForMember(m => m.MenuItems, options => options.Ignore())
                .ForMember(m => m.HelpIcons, options => options.Ignore())
                .ForMember(m => m.IsUserAdmin, options => options.Ignore());

            Mapper.CreateMap<Square, Models.Squares.IndexViewModel.SquareViewModel>()
                .ForMember(m => m.HelpIcons, options => options.Ignore());

            Mapper.CreateMap<Organization, Models.Organizations.DetailViewModel>()
                .ForMember(m => m.CurrentUserIsOrganizationAdministrator, options => options.Ignore())
                .ForMember(m => m.Messages, options => options.Ignore());
            Mapper.CreateMap<Organization, EditViewModel>()
                .ForMember(m => m.ActiveInDistricts, options => options.MapFrom(o => o.ActiveInDistricts.Select(d => d.DistrictId)))
                .ForMember(m => m.ActiveOrganizationThemes, options => options.MapFrom(o => o.Themes.Select(t => t.ThemeId)))
                .ForMember(m => m.DistrictsInCity, options => options.Ignore())
                .ForMember(m => m.OrganizationThemes, options => options.Ignore())
                .ForMember(m => m.Logo, options => options.Ignore());
            Mapper.CreateMap<District, DistrictViewModel>()
                .ForMember(m => m.Selected, options => options.Ignore())
                .ForMember(m => m.DistrictName, options => options.MapFrom(d => MapDistrictName(d)));
            Mapper.CreateMap<OrganizationTheme, OrganizationThemeViewModel>()
                .ForMember(m => m.Selected, options => options.Ignore());
            Mapper.CreateMap<Organization, Models.Organizations.MembersViewModel>()
                .ForMember(m => m.CurrentUserIsOrganizationAdministrator, options => options.Ignore())
                .ForMember(m => m.Members, options => options.Ignore());
            Mapper.CreateMap<Organization, Models.Organizations.ActivitiesViewModel>()
            .ForMember(m => m.ActivitiesModel, options => options.Ignore());
            Mapper.CreateMap<OrganizationMembership, Models.Organizations.MembersViewModel.MemberViewModel>()
                .ForMember(m => m.Email, options => options.Ignore())
                .ForMember(m => m.HasBeenInvited, options => options.UseValue(false));

            Mapper.CreateMap<OrganizationProduct, ProductViewModel>();
            Mapper.CreateMap<OrganizationService, ServiceViewModel>();

            Mapper.CreateMap<NeighborhoodMessage, PostMessageViewModel>()
                .ForMember(m => m.Image1, options => options.Ignore())
                .ForMember(m => m.Image2, options => options.Ignore())
                .ForMember(m => m.Image3, options => options.Ignore())
                .ForMember(m => m.Image4, options => options.Ignore())
                .ForMember(m => m.Image5, options => options.Ignore())
                .ForMember(m => m.File1, options => options.Ignore())
                .ForMember(m => m.ExpirationDateTimeHour, options => options.Ignore())
                .ForMember(m => m.ExpirationDateTimeMinute, options => options.Ignore())
                .ForMember(m => m.Expires, options => options.Ignore());

            Mapper.CreateMap<Activity, PostViewModel>()
                .ForMember(m => m.StartDateHour, options => options.Ignore())
                .ForMember(m => m.StartDateMinute, options => options.Ignore())
                .ForMember(m => m.EndDateHour, options => options.Ignore())
                .ForMember(m => m.EndDateMinute, options => options.Ignore())
                .ForMember(m => m.CircleId, options => options.Ignore());

            Mapper.CreateMap<IActivity, Models.Activities.IndexViewModel.ActivityViewModel>()
                .ForMember(m => m.Attending, options => options.Ignore());

            Mapper.CreateMap<Notification, NotificationViewModel>();

            Mapper.CreateMap<Setting, ChangeNotificationViewModel.Setting>();
            Mapper.CreateMap<UserSetting, ChangeNotificationViewModel.SettingValue>();

            Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// The map district name.
        /// </summary>
        /// <param name="district">
        /// The district.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MapDistrictName(District district)
        {
            var districtName = Regex.Replace(district.DistrictName, @"Wijk [0-9 ]+", string.Empty);

            return string.IsNullOrWhiteSpace(districtName) ? district.CityName : districtName;
        }
    }
}