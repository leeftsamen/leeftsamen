// <copyright file="Square.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;

    public class Square
    {
        public virtual int SquareId { get; set; }

        public virtual string Name { get; set; }

        public virtual int CoverColor { get; set; }

        public virtual Media CoverImage { get; set; }

        public virtual int? CoverImageId { get; set; }

        public virtual Media ProfileImage { get; set; }

        public virtual int? ProfileImageId { get; set; }

        public virtual List<SquareZipCode> ZipCodes { get; set; }

        public virtual List<SquareAdmin> Admins { get; set; }

        public virtual string InfoTitle { get; set; }

        public virtual string InfoText { get; set; }

        public virtual string ForumTitle { get; set; }

        public virtual string ForumText { get; set; }
    }
}
