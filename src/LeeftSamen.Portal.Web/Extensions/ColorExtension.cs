// <copyright file="ColorExtension.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Extensions
{
    using System.Drawing;

    /// <summary>
    /// The color extension.
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// The change brightness.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="brightness">
        /// The brightness.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color ChangeBrightness(this Color color, float brightness = 0)
        {
            var r = (float)color.R;
            var g = (float)color.G;
            var b = (float)color.B;

            if (brightness < 0)
            {
                brightness = 1 + brightness;
                r *= brightness;
                g *= brightness;
                b *= brightness;
            }
            else
            {
                r = ((255 - r) * brightness) + r;
                g = ((255 - g) * brightness) + g;
                b = ((255 - b) * brightness) + b;
            }

            return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
        }
    }
}