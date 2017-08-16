// <copyright file="ImageHelper.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Web;

    using LeeftSamen.Portal.Services.DTO;
    using LeeftSamen.Portal.Web.Extensions;
    using System.Drawing.Imaging;

    public static class ImageHelper
    {
        public static ImageDto GetResizedImage(HttpPostedFileBase file, int maxWith, int maxHeight)
        {
            try
            {
                if (!file.IsImage())
                {
                    return null;
                }

                using (var image = Image.FromStream(file.InputStream))
                {
                    var resizedImage = ResizeImage(image, maxWith, maxHeight);
                    resizedImage.FileName = file.FileName;

                    return resizedImage;
                }
            }
            catch (Exception e)
            {
                // This exception will be thrown when the inputstream is not a valid image.
                return null;
            }
        }

        public static ImageDto ResizeImage(Image sourceImage, int maxWidth, int maxHeight)
        {
            double resizeWidth;
            double resizeHeight;

            if (sourceImage.Width <= maxWidth && sourceImage.Height <= maxHeight)
            {
                // If the source image is smaller than maxWidth and maxHeight
                resizeWidth = sourceImage.Width;
                resizeHeight = sourceImage.Height;
            }
            else if (sourceImage.Width - maxWidth > sourceImage.Height - maxHeight)
            {
                resizeWidth = maxWidth;
                resizeHeight = sourceImage.Height * ((double)maxWidth / sourceImage.Width);
            }
            else
            {
                resizeHeight = maxHeight;
                resizeWidth = sourceImage.Width * ((double)maxHeight / sourceImage.Height);
            }

            var targetImage = new Bitmap((int)resizeWidth, (int)resizeHeight);
            using (var g = Graphics.FromImage(targetImage))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(sourceImage, new Rectangle(0, 0, targetImage.Width, targetImage.Height));
            }

            return new ImageDto { Image = targetImage, Format = sourceImage.RawFormat };
        }
    }
}