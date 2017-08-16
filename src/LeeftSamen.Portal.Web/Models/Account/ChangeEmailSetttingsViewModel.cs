// <copyright file="ChangeEmailSetttingsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    public class ChangeEmailSettingsViewModel
    {
        [Display(ResourceType = typeof(Label), Name = "ReceivesWeekMail")]
        public bool ReceivesWeekMail { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ReceivesNewMarketplaceItemMail")]
        public bool ReceivesNewMarketplaceItemMail { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ReceivesMarketplaceMail")]
        public bool ReceivesMarketplaceMail { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ReceivesCircleMessageMail")]
        public bool ReceivesCircleMessageMail { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ReceivesCircleJobMail")]
        public bool ReceivesCircleJobMail { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ReceivesCircleJobAssigned")]
        public bool ReceivesCircleJobAssigned { get; set; }
    }
}