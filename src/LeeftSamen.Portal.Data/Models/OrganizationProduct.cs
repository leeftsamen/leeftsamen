// <copyright file="OrganizationProduct.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    public class OrganizationProduct
    {
        public virtual string FullText { get; set; }

        public virtual string IntroductionText { get; set; }

        public virtual string Title { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual int OrganizationProductId { get; internal set; }
    }
}