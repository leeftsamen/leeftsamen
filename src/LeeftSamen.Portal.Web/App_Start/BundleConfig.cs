// <copyright file="BundleConfig.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web
{
    using System.Configuration;
    using System.Web.Optimization;

    /// <summary>
    /// The bundle config.
    /// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// The register bundles.
        /// </summary>
        /// <param name="bundles">
        /// The bundles.
        /// </param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js")
                    .Include("~/Scripts/jquery.unobtrusive-ajax.min.js")
                    .Include("~/Scripts/jquery.validate.min.js")
                    .Include("~/Scripts/jquery.validate.unobtrusive.js")
                    );
            bundles.Add(
                new ScriptBundle("~/bundles/jqueryui").Include(
                    "~/Scripts/jquery-ui-{version}.js",
                    "~/Scripts/jquery.ui.touch-punch.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(
                new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/bootstrap-colorselector.js")
                    .Include("~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include("~/Scripts/angular.js"));
            //bundles.Add(new ScriptBundle("~/bundles/font").Include("~/Scripts/font.js"));
            bundles.Add(
                new ScriptBundle("~/bundles/neighborhood").Include("~/Scripts/leaflet-{version}.js")
                    .Include("~/Scripts/neighborhood.js"));
            bundles.Add(
                new ScriptBundle("~/bundles/neighborhoodGmap").Include("~/Scripts/leaflet-{version}.js")
                    .Include("~/Scripts/neighborhoodGmap.js"));

            bundles.Add(new ScriptBundle("~/bundles/marketplace").Include("~/Scripts/leaflet-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include("~/Scripts/tinymce/tinymce.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/leeftsamen").Include("~/Scripts/leeftsamen.js"));

            bundles.Add(new ScriptBundle("~/bundles/item-tiler").Include("~/Scripts/item-tiler.js"));

            bundles.Add(new ScriptBundle("~/bundles/lightbox").Include("~/Scripts/lightbox/js/lightbox.min.js"));

            // StyleBundle with specific order
            var styleBundle =
                new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css")
                    .Include("~/Content/bootstrap-colorselector.css")
                    .Include("~/Content/font-awesome.min.css")
                    .Include("~/Content/themes/base/core.css")
                    .Include("~/Content/themes/base/datepicker.css")
                    .Include("~/Content/themes/base/slider.css")
                    .Include("~/Content/themes/base/theme.css")
                    .Include("~/Content/themes/base/autocomplete.css")
                    .Include(
                        string.Format(
                            "~/Content/{0}.css",
                            ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "restyle" : "site"));
            bundles.Add(styleBundle);

            bundles.Add(new StyleBundle("~/Content/leaflet").Include("~/Content/leaflet.css"));

            bundles.Add(new StyleBundle("~/Content/lightbox").Include("~/Scripts/lightbox/css/lightbox.css"));
        }
    }
}