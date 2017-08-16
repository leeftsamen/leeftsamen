// <copyright file="ActionsMenuModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Action
{
    using System.Collections.Generic;

    public class ActionsMenuModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexVoteModel"/> class.
        /// </summary>
        public ActionsMenuModel()
        {
            this.Menus = new List<ActionMenuItem>();
        }

        public List<ActionMenuItem> Menus { get; set; }

        public class ActionMenuItem
        {
            public int ActionId { get; set; }

            public string MenuItem { get; set; }
        }
    }
}