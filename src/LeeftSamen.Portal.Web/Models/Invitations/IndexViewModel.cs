// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Invitations
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<InvitationsViewModel> Invitations { get; set; }
    }
}
