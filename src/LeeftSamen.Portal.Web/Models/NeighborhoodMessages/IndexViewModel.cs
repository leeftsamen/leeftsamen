// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.NeighborhoodMessages
{
    using System.Collections.Generic;

    using AutoMapper.Internal;

    using LeeftSamen.Portal.Data.Models;

    using HelpIcon = LeeftSamen.Portal.Data.Models.HelpIcon;

    /// <summary>
    /// The index view model.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public NeighborhoodMessage.MessageTypes MessageType { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public List<NeighborhoodMessage> Messages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can create message.
        /// </summary>
        public bool UserCanCreateMessage { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }
    }
}