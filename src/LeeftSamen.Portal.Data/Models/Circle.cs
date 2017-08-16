// <copyright file="Circle.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;

    public class Circle
    {
        public virtual int CircleId { get; set; }

        public virtual int CoverColor { get; set; }

        public virtual Media CoverImage { get; set; }

        public virtual int? CoverImageId { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual string Description { get; set; }

        public virtual ICollection<CircleInvitation> Invitations { get; set; }

        public virtual bool IsPrivate { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }

        public virtual ICollection<CircleJoinRequest> JoinRequests { get; set; }

        public virtual ICollection<CircleMembership> Members { get; set; }

        public virtual ICollection<CircleMessage> Messages { get; set; }

        public virtual ICollection<CirclePhotoAlbum> CirclePhotoAlbums { get; set; }

        public virtual ICollection<CirclePhoto> Photos { get; set; }

        public virtual string Name { get; set; }

        public virtual DbGeography Position { get; set; }

        public virtual Media ProfileImage { get; set; }

        public virtual int? ProfileImageId { get; set; }
    }
}