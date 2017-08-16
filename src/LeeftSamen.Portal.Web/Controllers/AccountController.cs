// <copyright file="AccountController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>
namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Enums;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Services.DTO;
    using LeeftSamen.Portal.Web.Helpers;
    using LeeftSamen.Portal.Web.Managers;
    using LeeftSamen.Portal.Web.Models.Account;
    using LeeftSamen.Portal.Web.Models.Register;
    using LeeftSamen.Portal.Web.Utils;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAuthenticationManager authenticationManager;

        private readonly INotificationService notificationService;

        private readonly ApplicationSignInManager signInManager;

        private readonly ICircleService circleService;

        private readonly ISettingService settingService;

        private readonly IUserService userService;

        private readonly IHelpIconService helpIconService;

        private readonly ISharedService sharedService;

        private readonly ZuiderlingHelper zuiderlingHelper;

        public AccountController(
            ICurrentUserInformation currentUserInformation,
            IUserService userService,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authenticationManager,
            ICircleService circleService,
            INotificationService notificationService,
            ISettingService settingService,
            ISharedService sharedService,
            IHelpIconService helpIconService)
            : base(currentUserInformation)
        {
            this.userService = userService;
            this.signInManager = signInManager;
            this.authenticationManager = authenticationManager;
            this.circleService = circleService;
            this.notificationService = notificationService;
            this.helpIconService = helpIconService;
            this.settingService = settingService;
            this.sharedService = sharedService;
            this.zuiderlingHelper = new ZuiderlingHelper();
        }

        [HttpGet]
        public ActionResult RemoveAccount()
        {
            return this.View(string.Format("RemoveAccount{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), new RemoveAccountPostModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveAccount(RemoveAccountPostModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View("RemoveAccount", model);
            }
            var user = this.CurrentUser;
            var onlyAdminCircles = await this.circleService.UserCirclesWhereUserIsOnlyAdmin(user);
            if (onlyAdminCircles.Count > 0)
            {
                this.NotifyUserDanger(Alert.OnlyAdminOfCircle, string.Join(",", onlyAdminCircles.Select(c => c.Name)));
                return this.View("RemoveAccount", model);
            }

            MvcApplication.DeleteSession();
            this.authenticationManager.SignOut();

            await this.userService.RemoveUserAsync(user, this.circleService, this.PortalUrl);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangeName()
        {
            return this.View(string.Format("ChangeName{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), new ChangeNamePostModel { Name = this.CurrentUser.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeName(ChangeNamePostModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View(string.Format("ChangeName{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
            }

            await this.userService.ChangeNameAsync(this.CurrentUser, model.Name);

            this.NotifyUserSuccess(Alert.ChangeNameSuccess);
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> ChangeNotifications()
        {
            var model = new ChangeNotificationViewModel();
            var settings = await this.settingService.GetSettings();
            var values = await this.settingService.GetUserSettingsByUser(this.CurrentUser.Id);

            model.Settings = Mapper.Map<List<ChangeNotificationViewModel.Setting>>(settings);
            model.Values = Mapper.Map<List<ChangeNotificationViewModel.SettingValue>>(values);
            model.Names = settings.Where(s => s.Group == SettingHelper.MobileGroup || s.Group == SettingHelper.WebsiteGroup).Select(s => s.Name).Distinct().ToList();

            return this.View(string.Format("ChangeNotifications{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeNotifications(FormCollection forms)
        {
            var settings = await this.settingService.GetSettings();
            var websiteSettings = settings.Where(s => s.Group == SettingHelper.WebsiteGroup).ToList();
            var mobileSettings = settings.Where(s => s.Group == SettingHelper.MobileGroup).ToList();
            var values = await this.settingService.GetUserSettingsByUser(this.CurrentUser.Id);
            var names = settings.Where(s => s.Group == SettingHelper.MobileGroup || s.Group == SettingHelper.WebsiteGroup).Select(s => s.Name).Distinct().ToList();

            foreach (string name in names)
            {
                var mobileSetting = mobileSettings.FirstOrDefault(s => s.Name == name);
                if (mobileSetting != null)
                {
                    bool formValue = true;
                    if (forms["mobile-" + mobileSetting.SettingId] != null)
                    {
                        formValue = bool.Parse(forms["mobile-" + mobileSetting.SettingId].Split(',')[0]);
                        var mobileValue = values.FirstOrDefault(v => v.SettingId == mobileSetting.SettingId);
                        if (mobileValue != null)
                        {
                            await this.settingService.UpdateUserSettingValue(mobileValue, formValue.ToString().ToLower());
                        }
                        else
                        {
                            await this.settingService.AddUserSetting(mobileSetting.SettingId, this.CurrentUser.Id, formValue.ToString().ToLower());
                        }
                    }
                }

                var websiteSetting = websiteSettings.FirstOrDefault(s => s.Name == name);
                if (websiteSetting != null)
                {
                    bool formValue = true;
                    if (forms["website-" + websiteSetting.SettingId] != null)
                    {
                        formValue = bool.Parse(forms["website-" + websiteSetting.SettingId].Split(',')[0]);
                        var websiteValue = values.FirstOrDefault(v => v.SettingId == websiteSetting.SettingId);
                        if (websiteValue != null)
                        {
                            await this.settingService.UpdateUserSettingValue(websiteValue, formValue.ToString().ToLower());
                        }
                        else
                        {
                            await this.settingService.AddUserSetting(websiteSetting.SettingId, this.CurrentUser.Id, formValue.ToString().ToLower());
                        }
                    }
                }
            }

            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChangeUsername()
        {
            return this.View(string.Format("ChangeUsername{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), new ChangeUsernamePostModel { Username = this.CurrentUser.UserName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUsername(ChangeUsernamePostModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View(string.Format("ChangeUsername{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
            }

            if (model.Username != this.CurrentUser.UserName)
            {
                var storedUser = await this.userService.FindByEmailAsync(model.Username);
                if (storedUser == null)
                {
                    await this.userService.ChangeUsernameAsync(this.CurrentUser, model.Username);
                }
                else
                {
                    this.ModelState.AddModelError("Email", Error.EmailAddressTaken);
                    return this.View(string.Format("ChangeUsername{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
                }
            }

            this.NotifyUserSuccess(Alert.ChangeUsernameSuccess);
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChangeNeighborhood()
        {
            var model = new NeighborhoodViewModel
            {
                City = this.CurrentUser.City,
                PostalCode = this.CurrentUser.PostalCode,
                HouseNumber = this.CurrentUser.HouseNumber,
                Latitude = this.CurrentUser.Latitude,
                Longitude = this.CurrentUser.Longitude,
                NeighborhoodRadius = this.CurrentUser.NeighborhoodRadius,
                Street = this.CurrentUser.Street,
                ShowLocation = this.CurrentUser.ShowLocation
            };

            return this.View(string.Format("ChangeNeighborhood{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeNeighborhood(NeighborhoodViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await
                    this.userService.ChangeNeighborhoodAsync(
                        this.CurrentUser,
                        model.PostalCode,
                        model.HouseNumber,
                        model.Street,
                        model.City,
                        model.Latitude,
                        model.Longitude,
                        model.NeighborhoodRadius,
                        model.ShowLocation);

                this.NotifyUserSuccess(Alert.ChangeNeighborhoodSuccess);
                return this.RedirectToAction("Index");
            }

            this.SetStatusCode(400);
            model.HasError = true;
            return this.View(string.Format("ChangeNeighborhood{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public ActionResult ChangeEmailSettings()
        {
            return this.View(string.Format("ChangeEmailSettings{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), new ChangeEmailSettingsViewModel
            {
                ReceivesWeekMail = this.CurrentUser.ReceivesWeekMail,
                ReceivesCircleJobMail = this.CurrentUser.ReceivesCircleJobMail,
                ReceivesCircleMessageMail = this.CurrentUser.ReceivesCircleMessageMail,
                ReceivesMarketplaceMail = this.CurrentUser.ReceivesMarketplaceMail,
                ReceivesCircleJobAssigned = this.CurrentUser.ReceivesCircleJobAssigned,
                ReceivesNewMarketplaceItemMail = this.CurrentUser.ReceivesNewMarketplaceitemMail
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmailSettings(ChangeEmailSettingsViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View(string.Format("ChangeEmailSettings{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
            }

            this.CurrentUser.ReceivesWeekMail = model.ReceivesWeekMail;
            this.CurrentUser.ReceivesMarketplaceMail = model.ReceivesMarketplaceMail;
            this.CurrentUser.ReceivesCircleJobMail = model.ReceivesCircleJobMail;
            this.CurrentUser.ReceivesCircleMessageMail = model.ReceivesCircleMessageMail;
            this.CurrentUser.ReceivesCircleJobAssigned = model.ReceivesCircleJobAssigned;
            this.CurrentUser.ReceivesNewMarketplaceitemMail = model.ReceivesNewMarketplaceItemMail;

            await this.userService.ChangeEmailSettingsAsync(this.CurrentUser);

            this.NotifyUserSuccess(Alert.ChangeEmailSettingsSuccess);
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return this.View(string.Format("ChangePassword{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View(string.Format("ChangePassword{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
            }

            var result =
                await
                this.userService.ChangePasswordAsync(
                    this.User.Identity.GetUserId(),
                    model.OldPassword,
                    model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this.userService.GetUserByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.signInManager.SignInAsync(user, false, false);
                }

                this.NotifyUserSuccess(Alert.ChangePasswordSuccess);

                return this.RedirectToAction("Index");
            }

            this.SetStatusCode(400);
            this.AddErrors(result);
            return this.View(string.Format("ChangePassword{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> RotateProfileImage()
        {
            var profileImage = await this.userService.GetProfileImageByUserIdAsync(this.CurrentUser.Id, this.CurrentUser.ProfileImageId);

            if (profileImage != null)
            {
                using (var ms = new MemoryStream(profileImage.Data))
                {
                    var rawimage = Image.FromStream(ms);
                    rawimage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                    var image = new ImageDto
                    {
                        FileName = "rotated",
                        Format = new ImageFormat(ImageCodecInfo.GetImageEncoders().First(codec => codec.MimeType == profileImage.MimeType).FormatID),
                        Image = rawimage
                    };

                    await this.userService.SetProfileImageAsync(this.CurrentUser, image);
                }

                this.NotifyUserSuccess(Alert.ChangeProfilePictureSuccess);
            }

            return this.RedirectToAction("ChangeProfileImage");
        }

        [HttpGet]
        public ActionResult ChangeProfileImage()
        {
            return this.View(string.Format("ChangeProfileImage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), Mapper.Map<ChangeProfileImageViewModel>(this.CurrentUser));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfileImage(ChangeProfileImagePostModel model)
        {
            var resizedProfileImage = ImageHelper.GetResizedImage(model.ProfileImage, 100, 100);
            if (resizedProfileImage == null)
            {
                if (model.ProfileImage != null)
                {
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                }
                else
                {
                    this.ModelState.AddModelError("ProfileImage", Error.ProfileImageIsRequired);
                }

                this.SetStatusCode(HttpStatusCode.BadRequest);
                return this.View(string.Format("ChangeProfileImage{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), Mapper.Map<ChangeProfileImageViewModel>(this.CurrentUser));
            }

            await this.userService.SetProfileImageAsync(this.CurrentUser, resizedProfileImage);

            this.NotifyUserSuccess(Alert.ChangeProfilePictureSuccess);
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> ChangeZuiderlingSettings()
        {
            var allowZuiderling = await this.userService.AllowZuiderlingAsync(this.CurrentUser);
            if (!allowZuiderling)
            {
                return this.HttpForbidden();
            }

            var viewModel = new ChangeZuiderlingSettingsViewModel
            {
                AccountVerified = this.CurrentUser.HasZuiderlingAccount,
                ZA = this.CurrentUser.ZuiderlingAccount
            };

            if (string.IsNullOrEmpty(viewModel.ZA))
            {
                viewModel.ZA = " ";
            }

            return this.View(string.Format("ChangeZuiderlingSettings{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeZuiderlingSettings(ChangeZuiderlingSettingsViewModel model)
        {
            //return this.HttpNotFound();
            if (!this.CurrentUser.HasZuiderlingAccount)
            {
                var result = this.zuiderlingHelper.CheckCredentials(model.ZA, model.ZP);
                switch (result)
                {
                    case ZuiderlingHelper.AccountCredentialsStatus.Valid:
                        this.NotifyUserSuccess(Alert.ZuiderlingAccountLinked);
                        await this.userService.ChangeZuiderlingAccountAsync(this.CurrentUser, model.ZA);
                        break;
                    case ZuiderlingHelper.AccountCredentialsStatus.Invalid:
                        this.ModelState.AddModelError("Password", Error.ZuiderlingAccountWrong);
                        break;
                    case ZuiderlingHelper.AccountCredentialsStatus.Blocked:
                        this.ModelState.AddModelError("ZuiderlingAccount", Error.ZuiderlingAccountBlocked);
                        break;
                    case ZuiderlingHelper.AccountCredentialsStatus.Error:
                        this.ModelState.AddModelError("ZuiderlingAccount", Error.ZuiderlingAccountException);
                        break;
                }
            }

            var viewModel = new ChangeZuiderlingSettingsViewModel
            {
                AccountVerified = this.CurrentUser.HasZuiderlingAccount,
                ZA = this.CurrentUser.ZuiderlingAccount
            };
            return this.View(string.Format("ChangeZuiderlingSettings{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            if (userId == null || code == null)
            {
                return this.View("Error");
            }

            var result = await this.userService.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            return this.View("ForgotPassword", new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View("ForgotPassword", model);
            }

            var user = await this.userService.FindByNameAsync(model.Email);

            if (user != null)
            {
                await this.userService.SendResetPasswordLinkAsync(user, this.PortalUrl);

                this.NotifyUserSuccess(Text.ForgotPasswordInstructionsSend);

                return this.RedirectToAction("Login", "Account");
            }
            else if (user == null)
            {
                this.NotifyUserDanger(Text.ForgotPasswordInstructionError);
                return this.RedirectToAction("ForgotPassword", "Account");
            }

            return this.RedirectToAction("Login", "Account");
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var model = Mapper.Map<HeaderViewModel>(this.CurrentUser);
            if (this.CurrentUserInformation.OrganizationMembership != null)
            {
                model.CurrentOrganization =
                    Mapper.Map<HeaderViewModel.OrganizationViewModel>(
                        this.CurrentUserInformation.OrganizationMembership);
            }

            model.LatestNotifications =
                this.notificationService.GetLatestNotificationsForUserAsync(this.CurrentUser).Result;
            model.HelpIcons = this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser).Result;

            model.AllowZuiderling = this.userService.AllowZuiderling(this.CurrentUser);
            if (!string.IsNullOrEmpty(this.CurrentUser.ZuiderlingAccount))
            {
                model.HasZuiderling = true;

                if (this.Session["LastPollDate"] == null || this.Session["LastPollAmount"] == null || (DateTime)this.Session["LastPollDate"] < DateTime.Now.AddMinutes(-5))
                {
                    this.Session["LastPollAmount"] = ZuiderlingHelper.GetCurrentBalance(this.CurrentUser.ZuiderlingAccount);
                    this.Session["LastPollDate"] = DateTime.Now;
                }

                if (this.Session["LastPollAmount"] != null)
                {
                    model.ZuiderlingAmount = (decimal)this.Session["LastPollAmount"];
                }
            }

            return this.PartialView(string.Format("_Header{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var userId = this.User.Identity.GetUserId();
            var user = await this.userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return this.View("Error");
            }

            var model = new IndexViewModel();

            model.AllowZuiderling = await this.userService.AllowZuiderlingAsync(user);

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LinkTokenAndroid(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                if (this.Session["PushNotificationDeviceKnown"] as bool? != true)
                {
                    await this.userService.RegisterUserDevice(this.CurrentUser, token, DeviceType.Android);
                    this.ControllerContext.HttpContext.Response.StatusCode = 404;
                    this.Session["PushNotificationDeviceKnown"] = true;
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(FormCollection form)
        {
            MvcApplication.DeleteSession();
            this.authenticationManager.SignOut();

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            if (returnUrl != null)
            {
                if (returnUrl == this.Url.Action("requestnew", "organizations"))
                {
                    Utils.SetReturnUrlCookie(returnUrl, this.Request, this.Response);

                    return this.RedirectToAction("Index", "Register");
                }

                if (returnUrl.Contains("circles") && returnUrl.Contains("acceptinvitation"))
                {
                    Utils.SetReturnUrlCookie(returnUrl, this.Request, this.Response);
                }
                else
                {
                    var token = Utils.GetAcceptTokenFromReturnUrl(returnUrl);
                    if (token != null)
                    {
                        if (!this.userService.UserExistsWithAcceptToken(token))
                        {
                            Utils.SetReturnUrlCookie(returnUrl, this.Request, this.Response);

                            return this.RedirectToAction("Index", "Register");
                        }
                    }
                }
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // Always use a persistent cookie.
            var persistentLogin = true;// Utils.RequestedByApp(this.Request);

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result =
                await this.signInManager.PasswordSignInAsync(model.Email, model.Password, persistentLogin, false);
            switch (result)
            {
                case SignInStatus.Success:
                    this.NotifyUserSuccess(Alert.LoginSuccess);
                    return this.RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return this.View("Lockout");

                default:

                    // TODO: Add friendlier message
                    this.ModelState.AddModelError(string.Empty, Error.InvalidLogin);
                    return this.View(model);
            }
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var model = new MenuViewModel()
            {
                Activities = this.sharedService.GetNewActivitiesCount(this.CurrentUser),
                Circles = this.sharedService.GetNewCirclesActivityCount(this.CurrentUser),
                ForSale = this.sharedService.GetNewForSaleCount(this.CurrentUser),
                Meals = this.sharedService.GetNewMealsCount(this.CurrentUser),
                NeigborhoodMessages = this.sharedService.GetNewNeighborhoodMessageCount(this.CurrentUser),
                NeighborHelp = this.sharedService.GetNewNeighborHelpCount(this.CurrentUser),
                Organisations = this.sharedService.GetNewOrganisationsCount(this.CurrentUser),
                PublicCircles = this.sharedService.GetNewPublicCirclesCount(this.CurrentUser),
                ToBorrow = this.sharedService.GetNewToBorrowCount(this.CurrentUser)
            };
            return this.PartialView("_Menu", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            return code == null
                       ? this.View("Error")
                       : this.View("ResetPassword", new ResetPasswordViewModel { UserId = userId, Code = code });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string userId, string code, ResetPasswordViewModel model)
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            if (!this.ModelState.IsValid)
            {
                this.SetStatusCode(400);
                return this.View(model);
            }

            var user = await this.userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                this.SetStatusCode(400);
                return this.View(model);
            }

            var result = await this.userService.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                var signInResult =
                    await this.signInManager.PasswordSignInAsync(user.Email, model.Password, false, false);
                if (signInResult == SignInStatus.Success)
                {
                    this.NotifyUserSuccess(Alert.ResetPasswordSuccess);
                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.SetStatusCode(400);
            this.AddErrors(result);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectOrganization(int? membershipId, string returnUrl)
        {
            if (!membershipId.HasValue)
            {
                this.CurrentUserInformation.OrganizationMembership = null;
            }

            this.CurrentUserInformation.OrganizationMembership =
                await this.userService.GetUserOrganizationMembershipByIdAsync(this.CurrentUser, membershipId);
            if (this.CurrentUserInformation.OrganizationMembership != null)
            {
                this.NotifyUserSuccess(
                    Alert.SelectOrganizationSuccess,
                    this.CurrentUserInformation.OrganizationMembership.Organization.Name);
            }
            else
            {
                this.NotifyUserSuccess(Alert.DeselectOrganizationSuccess);
            }

            return this.RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public ActionResult SelectOrganization()
        {
            return this.RedirectToAction("Index", "Account");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
