// <copyright file="CreateReactionViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class CreateReactionViewModel : CreateReactionPostModel
    {
        public int SquareId { get; set; }

        public int SubjectId { get; set; }

        public static CreateReactionViewModel FromPostModel(CreateReactionPostModel model)
        {
            return new CreateReactionViewModel
            {
                Text = model.Text
            };
        }
    }
}