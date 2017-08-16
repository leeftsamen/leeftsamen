// <copyright file="SuggestionModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class SuggestionModel : BaseModel
    {
        public string SendByEmail { get; set; }

        public string SendByName { get; set; }

        public string SuggestionSubject { get; set; }

        public string Suggestion { get; set; }

        public override string TemplateName
        {
            get
            {
                return "Suggestion";
            }
        }
    }
}