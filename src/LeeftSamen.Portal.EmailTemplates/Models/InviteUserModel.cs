// <copyright file="InviteUserModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class InviteUserModel : BaseModel
    {
        public override string TemplateName
        {
            get
            {
                return "InviteMail";
            }
        }

        public string Message { get; set; }

        public string Reason { get; set; }
    }
}
