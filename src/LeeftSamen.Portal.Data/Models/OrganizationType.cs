// <copyright file="OrganizationType.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    public class OrganizationType
    {
        public enum Types
        {
            /// <summary>
            /// The professional.
            /// </summary>
            Professional = 0,

            /// <summary>
            /// The volunteer.
            /// </summary>
            Volunteer = 1,

            /// <summary>
            /// The association.
            /// </summary>
            Association = 2
        }

        public virtual string Name { get; set; }

        public virtual int OrganizationTypeId { get; set; }

        public virtual Types Type { get; set; }
    }
}