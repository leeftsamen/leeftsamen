// <copyright file="EnumExtensions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Extensions
{
    using System;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    /// <summary>
    /// The enum extensions.
    /// </summary>
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum value)
        {
            DisplayAttribute attr = value.GetDisplayAttribute();

            string outString = "";
            if (attr != null)
            {
                outString = attr.Name;
                if (attr.ResourceType != null)
                {
                    outString = attr.GetName();
                }
            }

            return outString;
        }

        public static DisplayAttribute GetDisplayAttribute(this Enum value)
        {
            DisplayAttribute attr = null;
            Type enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attrs != null && attrs.Any())
            {
                attr = ((DisplayAttribute)attrs[0]);
            }

            return attr;
        }
    }
}