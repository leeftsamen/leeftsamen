// <copyright file="SearchResult.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System.Collections.Generic;

    public class SearchResult
    {
        public enum Categories
        {
            Activities,

            Circles,

            Marketplace,

            NeighborhoodMessages
        }

        public Categories Category { get; set; }

        public int Identifier { get; set; }

        public string Label { get; set; }

        public int Score { get; set; }

        public KeyValuePair<string, string> ExtraData { get; set; }
    }
}