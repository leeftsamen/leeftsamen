// <copyright file="BootstrapHelper.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The bootstrap helper.
    /// </summary>
    public static class BootstrapHelper
    {
        /// <summary>
        /// The begin form group for.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// The type of the model
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property
        /// </typeparam>
        /// <returns>
        /// The <see cref="IHtmlString"/>.
        /// </returns>
        public static BootstrapFormGroup BeginFormGroupFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {
            return FormGroupHelper(htmlHelper, expression, htmlAttributes);
        }

        /// <summary>
        /// The form group helper.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// The type of the model.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the expression.
        /// </typeparam>
        /// <returns>
        /// The <see cref="BootstrapFormGroup"/>.
        /// </returns>
        private static BootstrapFormGroup FormGroupHelper<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("form-group");

            var fullHtmlFieldName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                    ExpressionHelper.GetExpressionText(expression));
            if (!htmlHelper.ViewData.ModelState.IsValidField(fullHtmlFieldName))
            {
                tagBuilder.AddCssClass("has-error");
            }

            if (htmlAttributes != null)
            {
                foreach (var property in htmlAttributes.GetType().GetProperties())
                {
                    var value = property.GetValue(htmlAttributes);
                    if (value != null)
                    {
                        if (property.Name == "class")
                        {
                            tagBuilder.AddCssClass(value.ToString());
                        }
                        else
                        {
                            tagBuilder.Attributes.Add(property.Name, value.ToString());
                        }
                    }
                }
            }

            return new BootstrapFormGroup(htmlHelper, tagBuilder);
        }

        /// <summary>
        /// The bootstrap form group.
        /// </summary>
        public class BootstrapFormGroup : IDisposable
        {
            /// <summary>
            /// The html helper.
            /// </summary>
            private readonly HtmlHelper htmlHelper;

            /// <summary>
            /// The tag builder.
            /// </summary>
            private readonly TagBuilder tagBuilder;

            /// <summary>
            /// Initializes a new instance of the <see cref="BootstrapFormGroup"/> class.
            /// </summary>
            /// <param name="htmlHelper">
            /// The html helper.
            /// </param>
            /// <param name="tagBuilder">
            /// The tag Builder.
            /// </param>
            public BootstrapFormGroup(HtmlHelper htmlHelper, TagBuilder tagBuilder)
            {
                this.htmlHelper = htmlHelper;
                this.tagBuilder = tagBuilder;

                htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            }

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                this.htmlHelper.ViewContext.Writer.Write(this.tagBuilder.ToString(TagRenderMode.EndTag));
            }
        }
    }
}