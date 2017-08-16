// <copyright file="PostalCodeViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Users
{
    public class PostalCodeViewModel
    {
        public bool IsInActiveCity { get; set; }

        public string PostalCode { get; set; }
    }
}