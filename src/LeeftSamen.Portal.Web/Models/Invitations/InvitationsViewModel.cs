// <copyright file="InvitationsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Invitations
{
    using System;

    public class InvitationsViewModel
    {
        public DateTime InvitationDateTime { get; set; }

        public int Id { get; set; }

        public string InvitedBy { get; set; }

        public string Name { get; set; }

        public string AcceptToken { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }
    }
}
