// <copyright file="CircleCreateEmailMessageModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class CircleCreateEmailMessageModel
    {
        public CircleCreateEmailMessageModel()
        {
            this.CheckedUsers = new List<string>();
        }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "SubjectRequired")]
        [Display(ResourceType = typeof(Label), Name = "Subject")]
        public string subjectText { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "MessageTextRequired")]
        [Display(ResourceType = typeof(Label), Name = "MessageText")]
        public string messageText { get; set; }

        public int CircleId { get; set; }

        public string CircleName { get; set; }

        public ICollection<CircleMembership> CircleUsers { get; set; }

        public User CurrentUser { get; set; }

        public List<string> CheckedUsers { get; set; }
    }
}
