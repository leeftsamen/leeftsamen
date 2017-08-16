// <copyright file="ChangeZipCodesModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class ChangeZipCodesModel
    {
        public int SquareId { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ZipCode")]
        public string NewZipCode { get; set; }

        public List<ZipCodeModel> Zipcodes { get; set; }

        public class ZipCodeModel
        {
            public int ZipcodeId { get; set; }

            public string ZipCode { get; set; }
        }
    }
}