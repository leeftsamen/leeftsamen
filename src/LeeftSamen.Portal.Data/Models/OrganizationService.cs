// <copyright file="OrganizationService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    public class OrganizationService
    {
        public virtual string FullText { get; set; }

        public virtual string IntroductionText { get; set; }

        public virtual string Title { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual int OrganizationServiceId { get; internal set; }
    }
}