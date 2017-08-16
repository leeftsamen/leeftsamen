// <copyright file="MenuItemModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models
{
    using System.Collections.Generic;
    using System.Web.Routing;

    /// <summary>
    /// The menu item model.
    /// </summary>
    public class MenuItemModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemModel"/> class.
        /// </summary>
        public MenuItemModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemModel"/> class.
        /// </summary>
        /// <param name="routeName">
        /// The route name.
        /// </param>
        /// <param name="routeValues">
        /// The route values.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="selected">
        /// The selected.
        /// </param>
        public MenuItemModel(string routeName, RouteValueDictionary routeValues, string text, bool selected = false, IEnumerable<MenuItemModel> subMenuItems = null)
        {
            this.RouteName = routeName;
            this.RouteValues = routeValues;
            this.Text = text;
            this.Selected = selected;
            this.SubMenuItems = subMenuItems ?? new List<MenuItemModel>();
        }

        public MenuItemModel(string text, string attributes, bool selected = false, IEnumerable<MenuItemModel> subMenuItems = null)
        {
            this.Text = text;
            this.Attributes = attributes;
            this.Selected = selected;
            this.SubMenuItems = subMenuItems ?? new List<MenuItemModel>();
        }

        /// <summary>
        /// Gets or sets the route name.
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Gets or sets the route values.
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        public string Attributes { get; set; }

        public IEnumerable<MenuItemModel> SubMenuItems { get; set; }
    }
}