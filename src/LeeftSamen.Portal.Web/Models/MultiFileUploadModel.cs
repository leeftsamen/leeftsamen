// <copyright file="MultiFileUploadModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System.Collections.Generic;
using System.Web.Routing;

namespace LeeftSamen.Portal.Web.Models
{
    public class MultiFileUploadModel
    {
        public MultiFileUploadModel(int? fileId, string routeName, RouteValueDictionary routeValues, string inputLabel, string inputName, List<Media> files = null)
        {
            this.FileId = fileId;
            this.RouteName = routeName;
            this.RouteValues = routeValues;
            if (this.RouteValues == null)
            {
                this.RouteValues = new RouteValueDictionary();
            }

            this.InputLabel = inputLabel;
            this.InputName = inputName;
            this.Files = files;
            if (this.Files == null)
            {
                this.Files = new List<Media>();
            }
        }

        public int? FileId { get; set; }

        public string InputLabel { get; set; }

        public string InputName { get; set; }

        public string RouteName { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

        public List<Media> Files { get; set; }
    }
}