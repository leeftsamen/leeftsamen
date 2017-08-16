// <copyright file="ReportAbuseModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class ReportAbuseModel : BaseModel
    {
        public string ForUrl { get; set; }

        public string ReportDescription { get; set; }

        public string SendByEmail { get; set; }

        public string SendByName { get; set; }

        public override string TemplateName
        {
            get
            {
                return "ReportAbuse";
            }
        }
    }
}