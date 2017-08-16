// <copyright file="CurrentUserMustBeInActiveCityExceptionAttribute.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Attributes
{
    using System;

    /// <summary>
    /// The current user must be in active city exception attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CurrentUserMustBeInActiveCityExceptionAttribute : Attribute
    {
    }
}