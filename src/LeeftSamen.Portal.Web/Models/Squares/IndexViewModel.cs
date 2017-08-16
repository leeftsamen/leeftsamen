// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Squares
{
    using System.Collections.Generic;
    using System.EnterpriseServices.Internal;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The circle index view model.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        public IndexViewModel()
        {
            this.Squares = new List<SquareViewModel>();
        }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        /// <summary>
        /// Gets or sets the user circles.
        /// </summary>
        public List<SquareViewModel> Squares { get; set; }

        /// <summary>
        /// The circle view model.
        /// </summary>
        public class SquareViewModel
        {
            public IEnumerable<HelpIcon> HelpIcons { get; set; }

            /// <summary>
            /// Gets or sets the circle id.
            /// </summary>
            public int SquareId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has profile image.
            /// </summary>
            public int? ProfileImageId { get; set; }
        }
    }
}