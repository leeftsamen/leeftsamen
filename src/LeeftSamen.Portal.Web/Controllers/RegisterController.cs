// <copyright file="RegisterController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.EmailTemplates.Models;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Managers;
    using LeeftSamen.Portal.Web.Models.Register;
    using LeeftSamen.Portal.Web.Utils;

    public class RegisterController : BaseController
    {
        private readonly ApplicationSignInManager signInManager;

        private readonly IUserService userService;

        public RegisterController(
            ICurrentUserInformation currentUserInformation,
            IUserService userService,
            ApplicationSignInManager signInManager)
            : base(currentUserInformation)
        {
            this.userService = userService;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public ActionResult Address()
        {
            var view = "Address";
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["Company"] == "comunios")
            {
                view = "AddressGmap";
            }

            return this.View(
                view,
                new AddressViewModel
                {
                    City = this.CurrentUser.City,
                    HouseNumber = this.CurrentUser.HouseNumber,
                    Latitude = this.CurrentUser.Latitude,
                    Longitude = this.CurrentUser.Longitude,
                    PostalCode = this.CurrentUser.PostalCode,
                    Street = this.CurrentUser.Street
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Address(AddressViewModel model)
        {
            var view = "Address";
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["Company"] == "comunios")
            {
                view = "AddressGmap";
            }

            if (!this.ModelState.IsValid)
            {
                model.HasError = true;
                return this.View(view, model);
            }

            await
                this.userService.ChangeNeighborhoodAsync(
                    this.CurrentUser,
                    model.PostalCode,
                    model.HouseNumber,
                    model.Street,
                    model.City,
                    model.Latitude,
                    model.Longitude,
                    2000,
                    this.CurrentUser.ShowLocation);

            return this.RedirectToAction("NeighborhoodRadius", "Register");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DeviceCallback(string email = null)
        {
            // This action lets the device know to open up the native login screen after registration.
            return this.Content(string.Empty);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Neighborhood()
        {
            return this.RedirectToActionPermanent("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegisterAccountViewModel model)
        {
            this.ViewBag.BackgroundPoster = true;

            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToStartPage();
            }

            if (!this.ModelState.IsValid || !model.AcceptTerms)
            {
                if (!model.AcceptTerms)
                {
                    this.ModelState.AddModelError("AcceptTerms", Error.AcceptTermsIsRequired);
                }

                return this.View("../Home/Index", model);
            }

            var storedUser = await this.userService.FindByEmailAsync(model.Email);
            if (storedUser == null)
            {
                var user =
                    await
                    this.userService.CreateUserAsync(
                        model.Name,
                        model.Email,
                        model.Password,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        0,
                        0,
                        0,
                        this.PortalUrl);

                if (user == null)
                {
                    return this.View("../Home/Index", model);
                }

                // Use a persistent cookie if the app is used.
                var persistentLogin = Utils.RequestedByApp(this.Request);

                await this.signInManager.SignInAsync(user, persistentLogin, false);
                return this.RedirectToAction("Address", "Register");
            }

            this.ModelState.AddModelError("Email", Error.EmailAddressTaken);

            return this.View("../Home/Index", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuickRegister(IndexRegisterAccountViewModel model)
        {
            model.Name = model.FirstName + " " + model.LastName;

            return await this.Index(model);
        }

        [HttpGet]
        public ActionResult NeighborhoodRadius()
        {
            return this.View(
                "NeighborhoodRadius",
                new NeighborhoodRadiusViewModel
                {
                    Latitude = this.CurrentUser.Latitude,
                    Longitude = this.CurrentUser.Longitude,
                    NeighborhoodRadius = this.CurrentUser.NeighborhoodRadius
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NeighborhoodRadius(NeighborhoodRadiusViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("NeighborhoodRadius", "Register", model);
            }

            await
                this.userService.ChangeNeighborhoodAsync(
                    this.CurrentUser,
                    this.CurrentUser.PostalCode,
                    this.CurrentUser.HouseNumber,
                    this.CurrentUser.Street,
                    this.CurrentUser.City,
                    this.CurrentUser.Latitude,
                    this.CurrentUser.Longitude,
                    model.NeighborhoodRadius,
                    this.CurrentUser.ShowLocation);

            if (await this.userService.UserCanBecomePioneerAsync(this.CurrentUser))
            {
                return this.RedirectToAction("BecomePioneer", "Register");
            }

            var returnUrl = Utils.CheckReturnUrlCookie(this.Request, this.Response);
            if (returnUrl != null)
            {
                return this.RedirectToLocal(returnUrl);
            }

            return this.RedirectToStartPage();
        }

        [HttpGet]
        public async Task<ActionResult> BecomePioneer()
        {
            if (!await this.userService.UserCanBecomePioneerAsync(this.CurrentUser))
            {
                return this.RedirectToStartPage();
            }

            return this.View(new BecomePioneerPostModel
            {
                MakeMePioneer = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BecomePioneer(BecomePioneerPostModel model)
        {
            if (!await this.userService.UserCanBecomePioneerAsync(this.CurrentUser))
            {
                return this.RedirectToStartPage();
            }

            if (model.MakeMePioneer)
            {
                await this.userService.MakeUserPioneerAsync(this.CurrentUser);
            }

            var returnUrl = Utils.CheckReturnUrlCookie(this.Request, this.Response);
            if (returnUrl != null)
            {
                return this.RedirectToLocal(returnUrl);
            }

            return this.RedirectToStartPage();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient)]
        public async Task<ActionResult> PostalCodeCheck(string postalCode, string houseNumber)
        {
            string pro6PpResult;
            bool failed = false;

            //Todo: TEMP
            try
            {
                pro6PpResult = await GetPro6PpData(postalCode, houseNumber);
            }
            catch (Exception e)
            {
                pro6PpResult = e.ToString();
                failed = true;
            }

            return this.Content(pro6PpResult, "application/json");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PostalCodeCheck()
        {
            return new EmptyResult();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient)]
        public async Task<ActionResult> FindAdress(string searchString)
        {
            string gmapResult;
            bool failed = false;

            try
            {
                gmapResult = await GetGMAPData(searchString);
            }
            catch (Exception e)
            {
                gmapResult = e.ToString();
                failed = true;
            }

            return this.Content(gmapResult, "application/json");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult FindAdress()
        {
            return new EmptyResult();
        }

        private static async Task<string> GetPro6PpData(string postalCode, string streetNumber)
        {
            postalCode = postalCode.Replace("\t", string.Empty).Replace(" ", string.Empty).ToUpper();

            if (Regex.IsMatch(streetNumber, @"^(?![A-z]*$)[a-zA-Z0-9]+$"))  // Allows only numeric or alphanumeric values
            {
                var streetNumberWithoutExtension =
                Regex.Match(streetNumber, @"[0-9]+").Value.Replace("\t", string.Empty).Replace(" ", string.Empty);

                var extension =
                    streetNumber.Replace(streetNumberWithoutExtension, string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace(" ", string.Empty)
                        .Replace("-", string.Empty);

                var webClient = new WebClient();
                webClient.QueryString.Add("auth_key", "xxxxxxxxxxxxxxxx");
                webClient.QueryString.Add("nl_sixpp", postalCode);
                webClient.QueryString.Add("streetnumber", streetNumberWithoutExtension);

                // if (!string.IsNullOrWhiteSpace(extension))
                // {
                // webClient.QueryString.Add("extension", extension);
                // }

                return await webClient.DownloadStringTaskAsync("https://api.pro6pp.nl/v1/autocomplete");
            }

            return "{\"status\": \"error\"}";
        }

        private static async Task<string> GetGMAPData(string searchstring)
        {
            //if (Regex.IsMatch(searchstring, @"^(?![A-z]*$)[a-zA-Z0-9]+$"))  // Allows only numeric or alphanumeric values
            //{
                var webClient = new WebClient();
                webClient.QueryString.Add("key", System.Web.Configuration.WebConfigurationManager.AppSettings["GMapKey"]);
                webClient.QueryString.Add("address", searchstring);

                return await webClient.DownloadStringTaskAsync("https://maps.googleapis.com/maps/api/geocode/json");
            //}

            return "{\"status\": \"error\"}";
        }
    }
}