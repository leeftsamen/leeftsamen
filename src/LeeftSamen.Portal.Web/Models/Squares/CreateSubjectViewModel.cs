// <copyright file="CreateSubjectViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class CreateSubjectViewModel : CreateSubjectPostModel
    {
        public int SquareId { get; set; }

        public static CreateSubjectViewModel FromPostModel(CreateSubjectPostModel model)
        {
            return new CreateSubjectViewModel
            {
                Title = model.Title,
                Description = model.Description
            };
        }
    }
}