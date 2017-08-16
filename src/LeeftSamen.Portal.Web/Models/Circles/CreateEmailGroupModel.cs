// <copyright file="CreateEmailGroupModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class CreateEmailGroupModel
    {
        public CreateEmailGroupModel()
        {
            this.CheckedUsers = new List<string>();
        }

        //[Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "NameOfGroupIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "GroupName")]
        public string Name { get; set; }

        public int CircleId { get; set; }

        public string CircleName { get; set; }

        public ICollection<CircleMembership> CircleUsers { get; set; }

        public User CurrentUser { get; set; }

        public List<string> CheckedUsers { get; set; }
    }
}