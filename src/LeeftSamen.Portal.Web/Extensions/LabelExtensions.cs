// <copyright file="LabelExtensions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Extensions
{
    using Common.InterfaceText;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web.Mvc;

    public static class LabelExtensions
    {
        public static MvcHtmlString RequiredTextLabel(this HtmlHelper html, string labelText)
        {
            return RequiredTextLabel(html, labelText, Label.DefaultRequiredSuffix);
        }

        public static MvcHtmlString RequiredTextLabel(this HtmlHelper html, string labelText, string requiredSuffix)
        {
            return new MvcHtmlString(String.Concat(labelText, requiredSuffix ?? Label.DefaultRequiredSuffix));
        }

        public static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression)
        {
            return RequiredLabel(html,
                         expression,
                         labelText: null,
                         requiredSuffix: null,
                         checkFieldIsRequired: null);
        }

        public static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression, string requiredSuffix)
        {
            return RequiredLabel(html,
                         expression,
                         labelText: null,
                         requiredSuffix: requiredSuffix,
                         checkFieldIsRequired: null);
        }

        public static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression, string requiredSuffix, bool? checkFieldIsRequired)
        {
            return RequiredLabel(html,
                         expression,
                         labelText: null,
                         requiredSuffix: requiredSuffix,
                         checkFieldIsRequired: checkFieldIsRequired);
        }

        public static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression, string labelText, string requiredSuffix, bool? checkFieldIsRequired)
        {
            return RequiredLabel(html, expression, labelText, requiredSuffix, checkFieldIsRequired, htmlAttributes: null);
        }

        public static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression, string requiredSuffix, bool? checkFieldIsRequired, object htmlAttributes)
        {
            return RequiredLabel(html, expression, labelText: null, requiredSuffix: requiredSuffix, checkFieldIsRequired: checkFieldIsRequired, htmlAttributes: htmlAttributes);
        }

        internal static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression, string labelText, string requiredSuffix, bool? checkFieldIsRequired, object htmlAttributes)
        {
            return RequiredLabel(html,
                         expression,
                         labelText,
                         requiredSuffix,
                         checkFieldIsRequired,
                         HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        internal static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression, string labelText, string requiredSuffix, bool? checkFieldIsRequired, IDictionary<string, object> htmlAttributes)
        {
            return LabelHelper(html,
                               ModelMetadata.FromStringExpression(expression, html.ViewData),
                               expression,
                               requiredSuffix,
                               checkFieldIsRequired,
                               labelText,
                               htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return RequiredLabelFor<TModel, TValue>(html, expression, requiredSuffix: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix)
        {
            return RequiredLabelFor<TModel, TValue>(html, expression, requiredSuffix: null, labelText: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix, string labelText)
        {
            return RequiredLabelFor<TModel, TValue>(html, expression, requiredSuffix: requiredSuffix, checkFieldIsRequired: null, labelText: labelText);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix, bool? checkFieldIsRequired)
        {
            return RequiredLabelFor<TModel, TValue>(html, expression, requiredSuffix: requiredSuffix, checkFieldIsRequired: checkFieldIsRequired, labelText: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix, bool? checkFieldIsRequired, string labelText)
        {
            return RequiredLabelFor(html, expression, requiredSuffix, checkFieldIsRequired, labelText, htmlAttributes: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return RequiredLabelFor<TModel, TValue>(html, expression, requiredSuffix: null, checkFieldIsRequired: null, labelText: null, htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix, bool? checkFieldIsRequired, object htmlAttributes)
        {
            return RequiredLabelFor(html, expression, requiredSuffix: requiredSuffix, checkFieldIsRequired: checkFieldIsRequired, labelText: null, htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix, bool? checkFieldIsRequired, string labelText, object htmlAttributes)
        {
            return RequiredLabelFor(html, expression, requiredSuffix, checkFieldIsRequired, labelText, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        internal static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string requiredSuffix, bool? checkFieldIsRequired, string labelText, IDictionary<string, object> htmlAttributesDic)
        {
            return LabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               requiredSuffix,
                               checkFieldIsRequired,
                               labelText,
                               htmlAttributesDic);
        }

        internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string requiredSuffix = null, bool? checkFieldIsRequired = true, string labelText = null, IDictionary<string, object> htmlAttributes = null)
        {
            string resolvedLabelText = labelText ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }

            if ( !(checkFieldIsRequired ?? true) || metadata.IsRequired)
            {
                resolvedLabelText += requiredSuffix ?? Label.DefaultRequiredSuffix;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tag.SetInnerText(resolvedLabelText);
            tag.MergeAttributes(htmlAttributes, replaceExisting: true);
            return tag.ToMvcHtmlString(TagRenderMode.Normal);
        }

        private static MvcHtmlString ToMvcHtmlString(this TagBuilder tag, TagRenderMode renderMode)
        {
            return MvcHtmlString.Create(tag.ToString(renderMode));
        }
    }
}