// <copyright file="ApplicationSignInManager.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Managers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;

    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    /// <summary>
    /// The application sign in manager.
    /// </summary>
    public class ApplicationSignInManager : SignInManager<User, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSignInManager"/> class.
        /// </summary>
        /// <param name="userManager">
        /// The user manager.
        /// </param>
        /// <param name="authenticationManager">
        /// The authentication manager.
        /// </param>
        public ApplicationSignInManager(
            UserService userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        /// <summary>
        /// The create user identity async.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((UserService)this.UserManager);
        }
    }
}