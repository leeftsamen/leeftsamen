// <copyright file="FileUploadModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models
{
    public class FileUploadModel
    {
        public FileUploadModel(int? fileId, string routeName, object routeValues, string inputLabel, string inputName)
        {
            this.FileId = fileId;
            this.RouteName = routeName;
            this.RouteValues = routeValues;
            this.InputLabel = inputLabel;
            this.InputName = inputName;
        }

        public int? FileId { get; set; }

        public string InputLabel { get; set; }

        public string InputName { get; set; }

        public string RouteName { get; set; }

        public object RouteValues { get; set; }
    }
}