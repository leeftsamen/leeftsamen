// <copyright file="CreateSubjectViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class CreateSubjectViewModel : CreateSubjectPostModel
    {
        public string Type { get; set; }

        public int TypeId { get; set; }

        public static CreateSubjectViewModel FromPostModel(CreateSubjectPostModel model)
        {
            return new CreateSubjectViewModel
            {
                Title = model.Title,
                Description = model.Description,
                Text = model.Text
            };
        }
    }
}