// <copyright file="HelpIconViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models
{
    public class HelpIconViewModel
    {
        public string HelpText { get; set; }

        public int Id { get; set; }

        public string Placement { get; set; }

        public string CssClass { get; set; }

        public enum HelpIconTextPlacement
        {
            Top = 1,
            Right = 2,
            Bottom = 3,
            Left = 4
        }
    }
}
