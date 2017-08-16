// <copyright file="BasicMailModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class BasicMailModel : BaseModel
    {
        public override string TemplateName
        {
            get
            {
                return "BasicMail";
            }
        }

        public string Message { get; set; }

        public string Reason { get; set; }
    }
}