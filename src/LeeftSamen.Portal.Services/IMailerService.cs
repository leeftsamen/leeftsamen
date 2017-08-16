// <copyright file="IMailerService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;

    using Microsoft.AspNet.Identity;

    public interface IMailerService : IIdentityMessageService
    {
        bool IsValidEmail(string email);

        Task SendAsync(IEmailTemplateModel model, params User[] users);

        Task SendAsync(IEmailTemplateModel model, string emailAddress);

        Task SendAsync(string emailAddress, string subject, string message);
    }
}