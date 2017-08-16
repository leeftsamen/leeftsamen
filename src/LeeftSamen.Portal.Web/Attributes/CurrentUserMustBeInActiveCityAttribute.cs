// <copyright file="CurrentUserMustBeInActiveCityAttribute.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Attributes
{
    using System;

    /// <summary>
    /// The current user must be in active city attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CurrentUserMustBeInActiveCityAttribute : Attribute
    {
    }
}