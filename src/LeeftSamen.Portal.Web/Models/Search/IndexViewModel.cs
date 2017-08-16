// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Search
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IndexViewModel()
        {
            this.Results = new List<Result>();
        }

        public string Query { get; set; }

        public List<Result> Results { get; set; }

        public string[] UniqueCategories { get; set; }

        public class Result
        {
            public string Category { get; set; }

            public string Label { get; set; }

            public string Url { get; set; }
        }
    }
}