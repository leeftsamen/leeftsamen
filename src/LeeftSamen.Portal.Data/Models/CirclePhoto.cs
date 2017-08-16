// <copyright file="CirclePhoto.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CirclePhoto
    {
        public virtual int CirclePhotoId { get; set; }

        public virtual Circle Circle { get; set; }

        public virtual User UploadedBy { get; set; }

        public virtual Media Photo { get; set; }

        public virtual int? PhotoId { get; set; }

        public virtual CirclePhotoAlbum CirclePhotoAlbum { get; set; }
    }
}
