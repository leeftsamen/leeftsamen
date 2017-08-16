// <copyright file="DecimalModelBinder.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Helpers
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;

    /// <summary>
    /// The decimal model binder.
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        /// <summary>
        /// The bind model.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="bindingContext">
        /// The binding context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                // if with period use InvariantCulture, if with comma use CurrentCulture
                actualValue = Convert.ToDecimal(
                    valueResult.AttemptedValue,
                    valueResult.AttemptedValue.Contains(".") ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}