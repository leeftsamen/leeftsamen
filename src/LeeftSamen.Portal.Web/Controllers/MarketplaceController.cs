// <copyright file="MarketplaceController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Web.ZuiderlingPayment;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.UI;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Attributes;
    using LeeftSamen.Portal.Web.Extensions;
    using LeeftSamen.Portal.Web.Helpers;
    using LeeftSamen.Portal.Web.Models.Marketplace;
    using LeeftSamen.Portal.Web.Utils;
    using System;
    using System.Configuration;

    /// <summary>
    /// The marketplace controller.
    /// </summary>
    [CurrentUserMustBeInActiveCity]
    public class MarketplaceController : BaseController
    {
        /// <summary>
        /// The circle service
        /// </summary>
        private readonly ICircleService circleService;

        private readonly IHelpIconService helpIconService;

        private readonly ISharedService sharedService;

        private readonly IUserService userService;

        /// <summary>
        /// The marketplace service.
        /// </summary>
        private readonly IMarketplaceService marketplaceService;

        private readonly ZuiderlingHelper zuiderlingHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketplaceController"/> class.
        /// </summary>
        /// <param name="currentUserInformation">
        /// The current User Information.
        /// </param>
        /// <param name="marketplaceService">
        /// The marketplace service.
        /// </param>
        /// <param name="helpIconService">
        /// </param>
        /// <param name="circleService">
        /// The circle Service.
        /// </param>
        public MarketplaceController(
            ICurrentUserInformation currentUserInformation,
            IMarketplaceService marketplaceService,
            IHelpIconService helpIconService,
            ICircleService circleService,
            ISharedService sharedService,
            IUserService userService)
            : base(currentUserInformation)
        {
            this.marketplaceService = marketplaceService;
            this.helpIconService = helpIconService;
            this.circleService = circleService;
            this.zuiderlingHelper = new ZuiderlingHelper();
            this.sharedService = sharedService;
            this.userService = userService;
        }

        /// <summary>
        /// The insert item.
        /// </summary>
        /// <param name="postedModel">
        /// The posted model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateItem(ItemPostModel postedModel, int? circleId)
        {
            if (this.ModelState.IsValid)
            {
                var image1 = ImageHelper.GetResizedImage(postedModel.Image1, 600, 600);
                var image2 = ImageHelper.GetResizedImage(postedModel.Image2, 600, 600);
                var image3 = ImageHelper.GetResizedImage(postedModel.Image3, 600, 600);
                var image4 = ImageHelper.GetResizedImage(postedModel.Image4, 600, 600);
                var image5 = ImageHelper.GetResizedImage(postedModel.Image5, 600, 600);

                if (
                    !(postedModel.Image1.IsImage(true) && postedModel.Image2.IsImage(true)
                      && postedModel.Image3.IsImage(true) && postedModel.Image4.IsImage(true)
                      && postedModel.Image5.IsImage(true)))
                {
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    var repostModel = ItemViewModel.FromPostModel(postedModel);
                    repostModel.CategoriesList = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
                    repostModel.CirleSelectListItems = await this.GetUserCirclesAsSelectList();
                    repostModel.Categories = await this.GetCategoriesAsSelectList(repostModel.CategoriesList);
                    repostModel.CircleId = circleId;
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    return this.View(string.Format("EditItem{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), repostModel);
                }

                var item = await
                    this.marketplaceService.CreateMarketplaceItemAsync(
                        postedModel.Title,
                        postedModel.Description,
                        postedModel.Currency,
                        postedModel.Price,
                        postedModel.PaymentOption,
                        postedModel.ShowInCircle ? postedModel.ShowInCircleId : default(int?),
                        postedModel.Type,
                        this.CurrentUser,
                        this.CurrentUserInformation.OrganizationMembership,
                        await
                        this.marketplaceService.GetMarketplaceItemCategoryByIdAsync(
                            postedModel.CategoryCategoryId.HasValue ? postedModel.CategoryCategoryId.Value : 0),
                        image1,
                        image2,
                        image3,
                        image4,
                        image5,
                        postedModel.AllowSharing,
                        postedModel.IsPublic,
                        postedModel.PreferenceDelivery,
                        postedModel.PreferenceMail,
                        postedModel.PreferenceOnline,
                        postedModel.PreferencePickup,
                        postedModel.ShowLocation,
                        postedModel.ExpirationDate,
                        this.PortalUrl);

                if (postedModel.ShowInCircle)
                {
                    var circle = await this.circleService.GetCircleByIdAsync(postedModel.ShowInCircleId);

                    if (circle != null)
                    {
                        var marketplaceItem = await this.marketplaceService.GetLatestOwnerMarketplaceItemsAsync(this.CurrentUser.Id);

                        await this.circleService.SendNotificationNewCircleMarketPlaceItem(circle, this.CurrentUser, marketplaceItem);
                        await this.circleService.SendEmailMarketPlaceItemCreatedToCircleMembers(circle, this.CurrentUser, this.PortalUrl, marketplaceItem);
                    }
                }
                else
                {
                }

                if (circleId.HasValue)
                {
                    return this.RedirectToRoute("Default", new { controller = "Marketplace", action = "Overview", CircleId = circleId });
                }
                else if (item.Category != null)
                {
                    return this.RedirectToRoute("Default", new { controller = "Marketplace", action = "Overview", Category = (int)item.Category.Alias });
                }

                return this.RedirectToRoute("Default", new { controller = "Marketplace", action = "MyMarketplace" });
            }

            this.SetStatusCode(HttpStatusCode.BadRequest);
            var model = ItemViewModel.FromPostModel(postedModel);
            model.CategoriesList = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
            model.Categories = await this.GetCategoriesAsSelectList(model.CategoriesList);
            model.CirleSelectListItems = await this.GetUserCirclesAsSelectList();
            model.AllowZuiderling = this.CurrentUser.HasZuiderlingAccount;
            model.CircleId = circleId;
            return this.View(string.Format("EditItem{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The create item.
        /// </summary>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="circleId">
        /// The circle Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> CreateItem(int? categoryId, int? circleId)
        {
            var categories = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
            MarketplaceItemCategory defaultCategory =
                categories.FirstOrDefault(c => c.Alias == MarketplaceItemCategory.CategoryAlias.Stuff);
            if (categoryId.HasValue)
            {
                var cat = await this.marketplaceService.GetMarketplaceItemCategoryByAliasAsync(categoryId.Value);
                if (cat != null)
                {
                    categoryId = cat.CategoryId;
                }
            }

            var model = new ItemViewModel
                            {
                                CategoriesList = categories,
                                Categories = await this.GetCategoriesAsSelectList(categories),
                                CirleSelectListItems = await this.GetUserCirclesAsSelectList(),
                                CategoryCategoryId =
                                    categoryId
                                    ?? (defaultCategory != null ? defaultCategory.CategoryId : default(int?)),
                                AllowSharing = true,
                                IsPublic = true,
                                CircleId = circleId
                            };
            if (categoryId.HasValue)
            {
                model.CategoryCategoryId = categoryId;
            }

            if (circleId.HasValue)
            {
                model.IsPublic = false;
                model.ShowInCircle = true;
                model.ShowInCircleId = circleId;

                var circle = await this.circleService.GetCircleByIdAsync(circleId);
                model.SocialVersion = this.circleService.GetCircleSettingByName(circle, "SocialVersion") != null;
                if (model.SocialVersion)
                {
                    var dinners = model.CategoriesList.FirstOrDefault(c => (int)c.Alias == 1);
                    if (dinners != null)
                    {
                        model.CategoryCategoryId = dinners.CategoryId;
                    }
                }
            }

            model.AllowZuiderling = this.CurrentUser.HasZuiderlingAccount;
            model.ShowLocation = this.CurrentUser.ShowLocation;
            model.Category = categories.FirstOrDefault(c => c.CategoryId == model.CategoryCategoryId);

            return this.View(string.Format("EditItem{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateCircleItem(int circleId)
        {
            this.ViewBag.DefaultCircle = circleId;

            return await this.CreateItem(categoryId: null, circleId: circleId);
        }

        /// <summary>
        /// The create reaction.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult CreateReaction(int? id, int? circleId)
        {
            var redirectUrl = circleId.HasValue ? this.Url.RouteUrl("CircleMarketplaceDetail", new { controller = "Marketplace", action = "Detail", id = id, circleId = circleId })
                        : this.Url.RouteUrl("DefaultDetail", new { controller = "Marketplace", action = "Detail", id = id });

            return this.Redirect(redirectUrl);
        }

        /// <summary>
        /// The create reaction.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="postedModel">
        /// The posted model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateReaction(int? id, int? circleId, ReactionPostModel postedModel)
        {
            var item =
                await
                this.marketplaceService.GetMarketplaceItemByIdAsync(
                    id,
                    this.CurrentUserPosition,
                    this.CurrentUserNeighborhoodRadius);
            if (item == null)
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                MarketplaceItemReaction parentReaction = null;
                if (postedModel.NewReactionParentId.HasValue)
                {
                    parentReaction =
                        await
                        this.marketplaceService.GetMarketplaceItemReactionByIdAsync(
                            item.MarketplaceItemId,
                            postedModel.NewReactionParentId.Value);
                }

                if (parentReaction == null
                    && (postedModel.NewReactionParentId.HasValue || this.CurrentUserCanEditItem(item)))
                {
                    return this.HttpNotFound();
                }

                var reaction =
                    await
                    this.marketplaceService.CreateReactionAsync(
                        item,
                        postedModel.NewReaction,
                        this.CurrentUser,
                        this.CurrentUserInformation.OrganizationMembership,
                        this.PortalUrl,
                        parentReaction);

                if (reaction != null)
                {
                    var redirectUrl = circleId.HasValue ? this.Url.RouteUrl("CircleMarketplaceDetail", new { controller = "Marketplace", action = "Detail", id = item.MarketplaceItemId, circleId = circleId })
                        : this.Url.RouteUrl("DefaultDetail", new { controller = "Marketplace", action = "Detail", id = item.MarketplaceItemId });

                    return this.Redirect(redirectUrl + "#reaction-" + reaction.ReactionId);
                }
            }

            var model =
                Mapper.Map<DetailViewModel>(
                    await
                    this.marketplaceService.GetMarketplaceItemWithPrivateReactionsByIdAsync(
                        item.MarketplaceItemId,
                        this.CurrentUser.Id,
                        this.CurrentUserPosition,
                        this.CurrentUserNeighborhoodRadius,
                        this.CurrentUserInformation.OrganizationMembership));
            model.ShownInCircle = circleId;
            model.CurrentUserCanEdit = this.CurrentUserCanEditItem(item);
            model.CurrentUserCanPlaceBaseReaction = !model.CurrentUserCanEdit;
            model.UserTransactionStatus = await this.GetTransactionStatus(item);
            return this.View(string.Format("Detail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The detail.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> Detail(int? id, int? circleId)
        {
            var data =
                await
                this.marketplaceService.GetMarketplaceItemWithPrivateReactionsByIdAsync(
                    id,
                    this.CurrentUser.Id,
                    this.CurrentUserPosition,
                    this.CurrentUserNeighborhoodRadius,
                    this.CurrentUserInformation.OrganizationMembership);
            if (data.MarketplaceItem == null)
            {
                return this.HttpNotFound();
            }

            var userCircles = await this.circleService.GetUserCirclesAsync(this.CurrentUser);
            var model = Mapper.Map<DetailViewModel>(data);
            model.CurrentUserCanVIew = model.MarketplaceItem.IsPublic
                                       || this.CurrentUser.Id == model.MarketplaceItem.Owner.Id
                                       || (model.MarketplaceItem.ShowInCircleId.HasValue
                                           && userCircles.Exists(
                                               c => c.CircleId == model.MarketplaceItem.ShowInCircleId));
            model.CurrentUserCanEdit = this.CurrentUserCanEditItem(model.MarketplaceItem);
            model.CurrentUserCanPlaceBaseReaction = this.CurrentUserCanPlaceBaseReaction(model.MarketplaceItem);
            model.UserTransactionStatus = await this.GetTransactionStatus(data.MarketplaceItem);
            model.ShownInCircle = circleId;
            model.AllowZuiderling = await this.userService.AllowZuiderlingAsync(this.CurrentUser);

            return this.View(string.Format("Detail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The edit item.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> EditItem(int? id, int? circleId)
        {
            var item = await this.marketplaceService.GetMarketplaceItemByIdAsync(id);
            if (item == null || !this.CurrentUserCanEditItem(item))
            {
                return this.HttpNotFound();
            }

            var model = Mapper.Map<ItemViewModel>(item);
            model.CategoriesList = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
            model.CirleSelectListItems = await this.GetUserCirclesAsSelectList();
            model.AllowZuiderling = this.CurrentUser.HasZuiderlingAccount;
            model.ExpirationDate = item.ExpirationDate;

            if (circleId.HasValue)
            {
                var circle = await this.circleService.GetCircleByIdAsync(circleId);
                model.SocialVersion = this.circleService.GetCircleSettingByName(circle, "SocialVersion") != null;
                if (model.SocialVersion)
                {
                    var dinners = model.CategoriesList.FirstOrDefault(c => (int)c.Alias == 1);
                }
            }

            return this.View(string.Format("EditItem{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The edit item.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="postedModel">
        /// The posted model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditItem(int? id, ItemPostModel postedModel)
        {
            var item = await this.marketplaceService.GetMarketplaceItemByIdAsync(id);
            if (item == null || !this.CurrentUserCanEditItem(item))
            {
                return this.HttpNotFound();
            }

            if (this.ModelState.IsValid)
            {
                if (
                    !(postedModel.Image1.IsImage(true) && postedModel.Image2.IsImage(true)
                      && postedModel.Image3.IsImage(true) && postedModel.Image4.IsImage(true)
                      && postedModel.Image5.IsImage(true)))
                {
                    this.SetStatusCode(HttpStatusCode.BadRequest);
                    var repostModel = ItemViewModel.FromPostModel(postedModel);
                    repostModel.CategoriesList = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
                    repostModel.Categories = await this.GetCategoriesAsSelectList(repostModel.CategoriesList);
                    repostModel.CirleSelectListItems = await this.GetUserCirclesAsSelectList();
                    repostModel.MarketplaceItemId = id;
                    repostModel.AllowZuiderling = this.CurrentUser.HasZuiderlingAccount;
                    this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
                    return this.View(string.Format("EditItem{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), repostModel);
                }

                var image1 = ImageHelper.GetResizedImage(postedModel.Image1, 600, 600);
                var image2 = ImageHelper.GetResizedImage(postedModel.Image2, 600, 600);
                var image3 = ImageHelper.GetResizedImage(postedModel.Image3, 600, 600);
                var image4 = ImageHelper.GetResizedImage(postedModel.Image4, 600, 600);
                var image5 = ImageHelper.GetResizedImage(postedModel.Image5, 600, 600);

                await
                    this.marketplaceService.UpdateMarketplaceItemAsync(
                        item,
                        postedModel.Title,
                        postedModel.Description,
                        postedModel.Currency,
                        postedModel.Price,
                        postedModel.PaymentOption,
                        postedModel.ShowInCircle ? postedModel.ShowInCircleId : default(int?),
                        postedModel.Type,
                        await
                        this.marketplaceService.GetMarketplaceItemCategoryByIdAsync(
                            postedModel.CategoryCategoryId.HasValue ? postedModel.CategoryCategoryId.Value : 0),
                        image1,
                        image2,
                        image3,
                        image4,
                        image5,
                        postedModel.AllowSharing,
                        postedModel.IsPublic,
                        postedModel.PreferenceDelivery,
                        postedModel.PreferenceMail,
                        postedModel.PreferenceOnline,
                        postedModel.PreferencePickup,
                        postedModel.ShowLocation,
                        postedModel.ExpirationDate);

                return this.RedirectToRoute("Default", new { controller = "Marketplace", action = "MyMarketplace" });
            }

            this.SetStatusCode(HttpStatusCode.BadRequest);
            var model = ItemViewModel.FromPostModel(postedModel);
            model.CategoriesList = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
            model.Categories = await this.GetCategoriesAsSelectList(model.CategoriesList);
            model.CirleSelectListItems = await this.GetUserCirclesAsSelectList();
            model.MarketplaceItemId = id;
            model.AllowZuiderling = this.CurrentUser.HasZuiderlingAccount;
            return this.View(string.Format("EditItem{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            this.ViewBag.Preview =
                await
                    this.marketplaceService.GetPublicMarketplaceItemsAsync(this.CurrentUserPosition,
                                             this.CurrentUserNeighborhoodRadius, 10);

            var categories = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
            var indexModel = new IndexViewModel
                                 {
                                     Categories = categories,
                                     Items =
                                         await
                                         this.marketplaceService.GetLatestMarketplaceItemsAsync(
                                             this.CurrentUserPosition,
                                             this.CurrentUserNeighborhoodRadius),
                                     CategoriesForSelect = await this.GetCategoriesAsSelectList(categories),
                                     HelpIcons =
                                         await
                                         this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                             this.CurrentUser)
                                 };

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), indexModel);
        }

        /// <summary>
        /// The item image.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="mediaId">
        /// The mediaId.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "crop;width;height")]
        public async Task<ActionResult> ItemImage(int? id, int? mediaId, int? index, int height = 600, int width = 600, bool crop = false)
        {
            var itemImage = await this.marketplaceService.GetMarketplaceItemImageAsync(id, mediaId, index);
            if (itemImage == null)
            {
                return this.HttpNotFound();
            }

            var image = new WebImage(itemImage.Data);
            image = image.Resize(height, width, true, true);

            if (crop)
            {
                if (image.Height < height)
                {
                    decimal calc = (decimal)height / (decimal)image.Height;
                    image = image.Resize(Convert.ToInt32(Math.Ceiling((decimal)image.Width * calc)), height);
                }

                if (image.Width < width)
                {
                    decimal calc = (decimal)width / (decimal)image.Width;
                    image = image.Resize(width, Convert.ToInt32(Math.Ceiling((decimal)image.Height * calc)));
                }

                int cropVert = (image.Height - height) / 2;
                int cropHor = (image.Width - width) / 2;
                cropVert = cropVert > 1 ? cropVert : 0;
                cropHor = cropHor > 1 ? cropHor : 0;
                image = image.Crop(cropVert, cropHor, cropVert, cropHor);
            }
            else
            {
                // We make sure the height is never larger than 2 times the width
                int cropMargin = image.Height - (image.Width * 2);
                if (cropMargin > 1)
                {
                    image = image.Crop(top: cropMargin / 2, bottom: cropMargin / 2);
                }
            }

            image = image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "crop;width;height")]
        public ActionResult PlaceholderItemImage()
        {
            var image = new WebImage(System.IO.File.ReadAllBytes(this.Server.MapPath("~/Content/Images/placeholder-image.png")));
            image = image.Resize(150, 100, true, true);
            image = image.Crop(1, 1, 1, 1); // border bugfix in WebImage
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        /// <summary>
        /// The my marketplace.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> MyMarketplace()
        {
            var model = new MyMarketplaceViewModel
                            {
                                MarketplaceItems =
                                    await
                                    this.marketplaceService.GetAllOwnerMarketplaceItemsAsync(
                                        this.CurrentUser.Id,
                                        this.CurrentUserInformation.OrganizationMembership)
                            };
            return this.View(string.Format("MyMarketPlace{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        /// <summary>
        /// The overview showing the marketplace items.
        /// </summary>
        /// <param name="searchModel">
        /// The searchModel.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> Overview(SearchModel searchModel)
        {
            // Validate Circle membership, if items for a circle are requested.
            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(searchModel.CircleId, out result))
            {
                return result;
            }

            string altText = string.Empty;
            if (searchModel.CircleId.HasValue)
            {
                var circle = await this.circleService.GetCircleByIdAsync(searchModel.CircleId.Value);
                var alternativeLabels = this.circleService.GetCircleLabels(circle);
                var label = alternativeLabels.FirstOrDefault(l => l.Label == "Title.WelcomeMarketplaceOverview");
                if (label != null)
                {
                    altText = label.Text;
                }
            }

            int? categoryId = null;
            if (searchModel.Category.HasValue)
            {
                var category = await this.marketplaceService.GetMarketplaceItemCategoryByAliasAsync(searchModel.Category.Value);
                if (category != null)
                {
                    categoryId = category.CategoryId;
                    if (!searchModel.CircleId.HasValue)
                    {
                        switch (category.Alias)
                        {
                            case MarketplaceItemCategory.CategoryAlias.Borrowing:
                                this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.ToBorrow);
                                break;
                            case MarketplaceItemCategory.CategoryAlias.HelpNeighborhood:
                                this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.NeighborHelp);
                                break;
                            case MarketplaceItemCategory.CategoryAlias.Meals:
                                this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.NewMeals);
                                break;
                            case MarketplaceItemCategory.CategoryAlias.Stuff:
                                this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.ForSale);
                                break;
                        }
                    }
                }
            }

            var items =
                await
                this.marketplaceService.GetMarketplaceItemsAsync(
                    this.CurrentUserPosition,
                    this.CurrentUserNeighborhoodRadius,
                    searchModel.Type,
                    categoryId,
                    searchModel.CircleId,
                    searchModel.Query,
                    searchModel.OrderBy,
                    searchModel.Skip,
                    searchModel.Take);

            if (this.ControllerContext.HttpContext.Request.IsAjaxRequest())
            {
                this.ViewData.Add("ListView", searchModel.ListView);
                this.ViewData.Add("CircleId", searchModel.CircleId);
                return this.PartialView("_OverviewItems", items);
            }

            var categories = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();

            var model = new OverviewViewModel
                            {
                                Take = searchModel.Take,
                                MarketplaceItems = items,
                                SelectedType = searchModel.Type,
                                Categories = await this.GetCategoriesAsSelectList(categories),
                                Category = searchModel.Category,
                                OrderByOptions = this.GetOrderByOptionsAsSelectList(),
                                ListView = searchModel.ListView,
                                SearchQuery = searchModel.Query,
                                CircleId = searchModel.CircleId,
                                CurrentCategory =
                                    searchModel.Category.HasValue
                                        ? categories.Where(c => (int)c.Alias == searchModel.Category)
                                              .Select(c => c.Name)
                                              .FirstOrDefault()
                                        : null,
                                Type = searchModel.Type,
                                OverviewText = altText,
                                HelpIcons =
                                    await
                                    this.helpIconService.GetNonShownHelpIconsForUserAsync(
                                        this.CurrentUser)
                            };

            if ((searchModel.Category == 3 || searchModel.Category == 0) && ConfigurationManager.AppSettings["ShowRestyle"] == "true")
            {
                return this.View("OverviewAddRestyle", model);
            }

            return this.View(string.Format("Overview{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        public async Task<ActionResult> QuickCreate(OverviewViewModel model)
        {
            var category = await this.marketplaceService.GetMarketplaceItemCategoryByAliasAsync(model.Category ?? 0);
            var item = await
                this.marketplaceService.CreateMarketplaceItemAsync(
                    model.ItemTitle,
                    model.ItemDescription,
                    MarketplaceItem.MarketplaceCurrency.Euro,
                    null,
                    null,
                    model.CircleId,
                    MarketplaceItem.MarketplaceItemType.Asked,
                    this.CurrentUser,
                    null,
                    category,
                    null,
                    null,
                    null,
                    null,
                    null,
                    false,
                    true,
                    false,
                    false,
                    false,
                    false,
                    false,
                    model.ExpirationDate,
                    this.PortalUrl);

            if ((int)category.Alias == 3)
            {
                await this.circleService.SendEmailLendMarketPlaceItemCreatedToNeighboorhood(this.CurrentUser, this.PortalUrl, item);
                this.NotifyUserSuccess(Alert.LendMarketplaceItem);
            }

            if ((int)category.Alias == 0)
            {
                await this.circleService.SendEmailHelpMarketPlaceItemCreatedToNeighboorhood(this.CurrentUser, this.PortalUrl, item);
                this.NotifyUserSuccess(Alert.HelpMarketplaceItem);
            }

            return this.RedirectToAction("Overview", model);
        }

        /// <summary>
        /// The remove item.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveItem(int? id)
        {
            var item = await this.marketplaceService.GetMarketplaceItemByIdAsync(id);
            if (item == null || !this.CurrentUserCanEditItem(item))
            {
                return this.HttpNotFound();
            }

            await this.marketplaceService.RemoveMarketplaceItemAsync(item);

            return this.RedirectToRoute("Default", new { controller = "Marketplace", action = "MyMarketplace" });
        }

        /// <summary>
        /// The remove item.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult RemoveItem()
        {
            return this.RedirectToRoute("Default", new { controller = "Marketplace", action = "MyMarketplace" });
        }

        /// <summary>
        /// The current user can edit item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CurrentUserCanEditItem(MarketplaceItem item)
        {
            return (this.CurrentUserInformation.OrganizationMembership != null && item.OrganizationMembershipId.HasValue
                    && this.CurrentUserInformation.OrganizationMembership.OrganizationMembershipId
                    == item.OrganizationMembershipId.Value
                    && this.CurrentUserInformation.OrganizationMembership.IsAdministrator)
                   || (this.CurrentUserInformation.OrganizationMembership == null
                       && !item.OrganizationMembershipId.HasValue && item.Owner.Id == this.CurrentUser.Id);
        }

        /// <summary>
        /// The current user can place base reaction.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CurrentUserCanPlaceBaseReaction(MarketplaceItem item)
        {
            if (this.CurrentUserInformation.OrganizationMembership != null)
            {
                return !this.CurrentUserCanEditItem(item)
                       && !item.Reactions.Any(
                           r =>
                           r.Creator.Id == this.CurrentUser.Id && r.OrganizationMembershipId.HasValue
                           && r.OrganizationMembershipId.Value
                           == this.CurrentUserInformation.OrganizationMembership.OrganizationMembershipId);
            }

            return !this.CurrentUserCanEditItem(item) && item.Reactions.All(r => r.Creator.Id != this.CurrentUser.Id);
        }

        /// <summary>
        /// The get categories as select list.
        /// </summary>
        /// <param name="categories">
        /// The categories.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<List<SelectListItem>> GetCategoriesAsSelectList(
            List<MarketplaceItemCategory> categories = null)
        {
            if (categories == null)
            {
                categories = await this.marketplaceService.GetAllMarketplaceItemCategoriesAsync();
            }

            var result =
                categories.Select(c => new SelectListItem { Value = string.Empty + c.CategoryId, Text = c.Name })
                    .ToList();

            result.Insert(0, new SelectListItem() { Text = Label.AllCategories, Value = string.Empty });

            return result;
        }

        /// <summary>
        /// The get OrderByOptions as select list.
        /// </summary>
        /// <returns>
        /// The list.
        /// </returns>
        private List<SelectListItem> GetOrderByOptionsAsSelectList()
        {
            var result = new List<SelectListItem>();
            result.Add(
                new SelectListItem()
                    {
                        Text = Label.OrderByDate,
                        Value = MarketplaceService.ItemOrderByOption.Date.ToString()
                    });
            result.Add(
                new SelectListItem()
                    {
                        Text = Label.OrderByDistance,
                        Value = MarketplaceService.ItemOrderByOption.Distance.ToString()
                    });
            result.Add(
                new SelectListItem()
                    {
                        Text = Label.OrderByPrice,
                        Value = MarketplaceService.ItemOrderByOption.Price.ToString()
                    });
            result.Add(
                new SelectListItem()
                    {
                        Text = Label.OrderByPriceDesc,
                        Value = MarketplaceService.ItemOrderByOption.PriceDesc.ToString()
                    });
            return result;
        }

        private async Task<List<SelectListItem>> GetUserCirclesAsSelectList(List<Circle> circles = null)
        {
            if (circles == null)
            {
                circles = await this.circleService.GetUserCirclesAsync(this.CurrentUser);
            }

            var result =
                circles.Select(c => new SelectListItem { Value = string.Empty + c.CircleId, Text = c.Name }).ToList();
            return result;
        }

        private bool CurrentUserHasCirclePermissions(int? circleId, out ActionResult result)
        {
            if (circleId.HasValue)
            {
                // No async/await here, because it's not supported by ASP.NET MVC in childactions.
                var circle = this.circleService.GetCircleByIdAsync(circleId).Result;
                if (circle == null)
                {
                    result = this.HttpNotFound();
                    return false;
                }

                var userIsMemberOfCircle = this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser)
                                           && !this.CurrentUserCanOnlyView();
                if (!userIsMemberOfCircle)
                {
                    result = this.HttpForbidden();
                    return false;
                }
            }

            result = null;
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Transaction(int? marketplaceItemId, int? circleId)
        {
            var marketplaceItem = await this.marketplaceService.GetMarketplaceItemByIdAsync(marketplaceItemId);
            if (marketplaceItem == null)
            {
                return this.HttpNotFound();
            }

            if (marketplaceItem.Price.HasValue)
            {
                var result =
                    await
                        this.zuiderlingHelper.DoPaymentAsync(this.CurrentUser.ZuiderlingAccount,
                            marketplaceItem.Owner.ZuiderlingAccount, marketplaceItem.Price.Value);
                switch (result.@return.status)
                {
                    case paymentStatus.NOT_ENOUGH_CREDITS:
                        this.NotifyUserDanger(Alert.ZuiderlingTransactionShortage);
                        break;
                    case paymentStatus.MAX_DAILY_AMOUNT_EXCEEDED:
                        this.NotifyUserDanger(Alert.ZuiderlingTransactionDailyLimit);
                        break;
                    case paymentStatus.RECEIVER_UPPER_CREDIT_LIMIT_REACHED:
                        this.NotifyUserDanger(Alert.ZuiderlingTransactionReceiverLimit);
                        break;
                    case paymentStatus.PROCESSED:
                        this.NotifyUserSuccess(Alert.ZuiderlingTransactionSuccess);
                        await this.marketplaceService.CreateMarketplaceItemTransactionAsync(marketplaceItem, this.CurrentUser, marketplaceItem.Owner, this.PortalUrl);
                        break;
                    default:
                        this.NotifyUserDanger(Alert.ZuiderlingTransactionError);
                        break;
                }
            }

            return this.RedirectToAction("Detail", new { id = marketplaceItemId, circleId = circleId });
        }

        private async Task<DetailViewModel.TransactionStatus> GetTransactionStatus(MarketplaceItem item)
        {
            if (item.Owner == this.CurrentUser)
            {
                return DetailViewModel.TransactionStatus.MarketplaceItemOwner;
            }
            else if (!this.CurrentUser.HasZuiderlingAccount)
            {
                return DetailViewModel.TransactionStatus.NoZuiderlingAccount;
            }
            else
            {
                var transaction =
                    await
                        this.marketplaceService.GetMarketplaceItemTransactionForMarketplaceItemAsync(item.MarketplaceItemId,
                            this.CurrentUser.Id, item.Owner.Id);

                if (transaction == null)
                {
                    return DetailViewModel.TransactionStatus.NotPaid;
                }
                else
                {
                    return DetailViewModel.TransactionStatus.Paid;
                }
            }
        }
    }
}