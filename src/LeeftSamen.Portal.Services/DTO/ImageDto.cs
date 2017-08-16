// <copyright file="ImageDto.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services.DTO
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageDto
    {
        public string FileName { get; set; }

        public ImageFormat Format { get; set; }

        public Image Image { get; set; }
    }
}