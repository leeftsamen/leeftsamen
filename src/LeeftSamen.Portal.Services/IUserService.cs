// <copyright file="IUserService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.EmailTemplates.Models;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;

    using Microsoft.AspNet.Identity;

    public interface IUserService : IDisposable
    {
        IClaimsIdentityFactory<User, string> ClaimsIdentityFactory { get; set; }

        TimeSpan DefaultAccountLockoutTimeSpan { get; set; }

        IIdentityMessageService EmailService { get; set; }

        int MaxFailedAccessAttemptsBeforeLockout { get; set; }

        IPasswordHasher PasswordHasher { get; set; }

        IIdentityValidator<string> PasswordValidator { get; set; }

        bool SupportsUserClaim { get; }

        bool SupportsUserEmail { get; }

        bool SupportsUserLockout { get; }

        bool SupportsUserLogin { get; }

        bool SupportsUserPassword { get; }

        bool SupportsUserPhoneNumber { get; }

        bool SupportsUserRole { get; }

        bool SupportsUserSecurityStamp { get; }

        bool SupportsUserTwoFactor { get; }

        IDictionary<string, IUserTokenProvider<User, string>> TwoFactorProviders { get; }

        bool UserLockoutEnabledByDefault { get; set; }

        IUserTokenProvider<User, string> UserTokenProvider { get; set; }

        IIdentityValidator<User> UserValidator { get; set; }

        Task<IdentityResult> AccessFailedAsync(string userId);

        Task<IdentityResult> AddClaimAsync(string userId, Claim claim);

        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Task<IdentityResult> AddPasswordAsync(string userId, string password);

        Task<IdentityResult> AddToRoleAsync(string userId, string role);

        Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles);

        bool AllowZuiderling(User user);

        Task<bool> AllowZuiderlingAsync(User user);

        Task ChangeNameAsync(User user, string name);

        Task ChangeNeighborhoodAsync(
            User user,
            string postalCode,
            string houseNumber,
            string street,
            string city,
            decimal latitude,
            decimal longitude,
            int neighborhoodRadius,
            bool showLocation);

        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token);

        Task ChangeUsernameAsync(User user, string username);

        Task ChangeZuiderlingAccountAsync(User user, string zuiderlingAccount);

        Task ChangeEmailSettingsAsync(User user);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType);

        Task<User> CreateUserAsync(
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
            string portalUrl);

        Task<IdentityResult> DeleteAsync(User user);

        Task<User> FindAsync(string userName, string password);

        Task<User> FindAsync(UserLoginInfo login);

        Task<User> FindByEmailAsync(string email);

        Task<User> FindByNameAsync(string userName);

        Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber);

        Task<string> GenerateEmailConfirmationTokenAsync(string userId);

        Task<string> GenerateUserTokenAsync(string purpose, string userId);

        Task<int> GetAccessFailedCountAsync(string userId);

        Task<IList<Claim>> GetClaimsAsync(string userId);

        Task<string> GetEmailAsync(string userId);

        Task<bool> GetLockoutEnabledAsync(string userId);

        Task<DateTimeOffset> GetLockoutEndDateAsync(string userId);

        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);

        Task<List<User>> GetAllUsersAsync();

        Task<string> GetPhoneNumberAsync(string userId);

        Task<Media> GetProfileImageByUserIdAsync(string userId, int? mediaId);

        Task<IList<string>> GetRolesAsync(string userId);

        Task<string> GetSecurityStampAsync(string userId);

        Task<bool> GetTwoFactorEnabledAsync(string userId);

        Task<User> GetUserByIdAsync(string userId);

        Task<OrganizationMembership> GetUserOrganizationMembershipByIdAsync(User user, int? organizationMembershipId);

        Task<List<Data.Models.Action>> GetUserActionsByIdAsync(User user);

        Task<List<User>> GetUsersInRangeAsync(decimal latitude, decimal longitude, int radiusInMeters);

        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);

        Task<bool> HasPasswordAsync(string userId);

        Task<bool> IsEmailConfirmedAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> IsLockedOutAsync(string userId);

        Task<bool> IsPhoneNumberConfirmedAsync(string userId);

        Task<bool> IsUserInActiveCity(User user);

        Task MakeUserPioneerAsync(User user);

        Task<IdentityResult> NotifyTwoFactorTokenAsync(string userId, string twoFactorProvider, string token);

        void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<User, string> provider);

        Task<IdentityResult> RemoveClaimAsync(string userId, Claim claim);

        Task<IdentityResult> RemoveFromRoleAsync(string userId, string role);

        Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles);

        Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login);

        Task<IdentityResult> RemovePasswordAsync(string userId);

        Task RemoveUserAsync(User user, ICircleService circleService, string portalUrl);

        Task<IdentityResult> ResetAccessFailedCountAsync(string userId);

        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        Task SendEmailAsync(string userId, string subject, string body);

        Task SendResetPasswordLinkAsync(User user, string portalUrl);

        Task CustomNotifyByEmailAsync(User user, BasicMailModel model);

        Task CustomNotifyByEmailAsync(string emailAddress, BasicMailModel model);

        Task SendInvitation(User invitedBy, List<string> emailAdresses, string message, string portalUrl);

        Task SetProfileImageAsync(User user, ImageDto image);

        Task RegisterUserDevice(User user, string token, DeviceType type);

        Task<List<UserDevice>> GetActiveUserDevices(User user);

        Task<List<UserDevice>> GetAllUserDevices(User user);

        Task<IdentityResult> UpdateAsync(User user);

        Task<User> UpdateUserLastSeenAsync(User user);

        Task<bool> UserCanBecomePioneerAsync(User user);

        bool UserExistsWithAcceptToken(string token);

        Task<bool> VerifyUserTokenAsync(string userId, string purpose, string token);
    }
}