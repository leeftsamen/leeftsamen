// <copyright file="CirclePhotoAlbum.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CirclePhotoAlbum
    {
        public virtual int CirclePhotoAlbumId { get; set; }

        public virtual string Title { get; set; }

        public virtual Circle Circle { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual ICollection<CirclePhoto> Photos { get; set; }
    }
}
