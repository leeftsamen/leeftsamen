// <copyright file="SuggestionModalViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LeeftSamen.Portal.Web.Models.Suggestion
{
    public class SuggestionModalViewModel
    {
        [Display(Name = "Onderwerp")]
        public IEnumerable<SelectListItem> SubjectListItems
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem { Text = "Buurtnieuws" },
                    new SelectListItem { Text = "Buurtagenda" },
                    new SelectListItem { Text = "Vraag en aanbod" },
                    new SelectListItem { Text = "Organisaties" },
                    new SelectListItem { Text = "Kringen" },
                    new SelectListItem { Text = "Instructievideo's" },
                    new SelectListItem { Text = "Website algemeen", Selected = true },
                    new SelectListItem { Text = "App" },
                    new SelectListItem { Text = "Overig" }
                };
            }
        }

        public string Subject { get; set; }

        [Display(Name = "Suggestie")]
        [Required]
        public string Suggestion { get; set; }
    }
}