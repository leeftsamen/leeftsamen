// <copyright file="CreateFactViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class CreateFactViewModel : CreateFactPostModel
    {
        public int SquareId { get; set; }

        public static CreateFactViewModel FromPostModel(CreateFactPostModel model)
        {
            return new CreateFactViewModel
            {
                Title = model.Title,
                IntroductionText = model.IntroductionText,
                FullText = model.FullText
            };
        }
    }
}