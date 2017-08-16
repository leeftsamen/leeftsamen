// <copyright file="UsersController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI;
    using System.Collections.Generic;

    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Users;
    using LeeftSamen.Portal.Web.Utils;
    using System.IO;
    using System.Linq;
    using Common.InterfaceText;
    using System.Configuration;/// <summary>
                               /// The users controller.
                               /// </summary>
    public class UsersController : BaseController
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="currentUserInformation">
        /// The current User Information.
        /// </param>
        /// <param name="userService">
        /// The user service.
        /// </param>
        public UsersController(ICurrentUserInformation currentUserInformation, IUserService userService)
            : base(currentUserInformation)
        {
            this.userService = userService;
        }

        [ChildActionOnly]
        public ActionResult PostalCode()
        {
            return this.PartialView(
                "_PostalCode",
                new PostalCodeViewModel
                    {
                        PostalCode = this.CurrentUser.PostalCode,
                        IsInActiveCity = this.CurrentUserInformation.IsUserInActiveCity
                    });
        }

        /// <summary>
        /// The profile image.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="mediaId">
        /// The media Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> ProfileImage(string userId, int? mediaId)
        {
            //var user = await this.userService.GetUserByIdAsync(userId);
            //if (user == null || user.ProfileImageId != mediaId)
            //{
            //    return this.HttpNotFound();
            //}

            var profileImage = await this.userService.GetProfileImageByUserIdAsync(userId, mediaId);
            if (profileImage == null)
            {
                return this.HttpNotFound();
            }

            return this.ResizedProfileImage(profileImage.Data);
        }

        [HttpGet]
        public ActionResult CurrentUserProfileImage()
        {
            if (this.CurrentUser.ProfileImage == null)
            {
                //var dir = Server.MapPath("/Content/Images");
                //var path = Path.Combine(dir, "profile-image.jpg");
                return this.File("~/Content/Images/profile-image.jpg", "image/jpeg");
            }

            return this.ResizedProfileImage(this.CurrentUser.ProfileImage.Data);
        }

        [HttpGet]
        public ActionResult Invite()
        {
            var model = new InviteModel();
            model.EmailText = string.Format(@"Hallo!

Graag wil ik je uitnodigen voor LeeftSamen, het digitale platform voor de buurt. Ik doe al mee. Samen maken we de beste buurt!

Via LeeftSamen kom je te weten wat er speelt in de buurt. Je kunt met anderen in contact komen en op een leuke, laagdrempelige manier iets voor elkaar betekenen. 

Een profiel aanmaken is gratis en eenvoudig. Zo kunnen we gemakkelijk samenwerken aan een sociale, gezellige en veilige wijk!

Groeten,
{0}", this.CurrentUser.Name);
            return this.View(string.Format("Invite{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Invite(InviteModel model)
        {
            var addresses = this.SeparateEmailAddresses(model.EmailAdresses).ToList();
            var filteredAddresses = addresses.Where(x => Utils.IsValidEmail(x)).ToList();
            if (filteredAddresses.Any())
            {
                await this.userService.SendInvitation(this.CurrentUser, filteredAddresses, model.EmailText, this.PortalUrl);
            }
            else
            {
                return this.View(model);
            }

            this.NotifyUserSuccess(Alert.UsersInvitedforWebsite);
            return this.Redirect(this.Url.RouteUrl("Default", new { controller = "Home", action = "Index" }));
        }
    }
}