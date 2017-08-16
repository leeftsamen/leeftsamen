// <copyright file="MailerService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates;
    using LeeftSamen.Portal.EmailTemplates.Models;

    using Microsoft.AspNet.Identity;

    public class MailerService : IMailerService
    {
        private readonly IMessageGenerator messageGenerator;

        private readonly SmtpClient smtpClient;

        public MailerService(SmtpClient smtpClient, IMessageGenerator messageGenerator)
        {
            this.smtpClient = smtpClient;
            this.messageGenerator = messageGenerator;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendAsync(string emailAddress, string subject, string message)
        {
            var model = new BasicMailModel { Subject = subject, Message = message.Replace("\n", "<br/>"), Reason = string.Empty };
            var mail = this.messageGenerator.GenerateMessage(model);
            mail.To.Add(emailAddress);

            await this.SendAsync(mail).ConfigureAwait(false);
        }

        public async Task SendAsync(IEmailTemplateModel model, params User[] users)
        {
            if (users.Length == 0)
            {
                return;
            }

            foreach (var user in users)
            {
                model.Receiver = user.Name;
                await this.SendAsync(this.messageGenerator.GenerateMessage(model), user).ConfigureAwait(false);
            }
        }

        public async Task SendAsync(IEmailTemplateModel model, string emailAddress)
        {
            var message = this.messageGenerator.GenerateMessage(model);
            message.To.Add(emailAddress);

            await this.SendAsync(message).ConfigureAwait(false);
        }

        public Task SendAsync(IdentityMessage message)
        {
            throw new NotSupportedException("Use: SendAsync(User user, IEmailTemplateModel model)");
        }

        private async Task SendAsync(MailMessage message, params User[] users)
        {
            foreach (var mailAddress in users.Select(user => new MailAddress(user.Email, user.Name, Encoding.UTF8)))
            {
                //if (message.To.Any())
                //{
                //    message.Bcc.Add(mailAddress);
                //}
                //else
                //{
                //    message.To.Add(mailAddress);
                //}
                message.To.Clear();
                message.To.Add(mailAddress);
                await this.SendAsync(message).ConfigureAwait(false);
            }
        }

        private async Task SendAsync(MailMessage message)
        {
            await this.smtpClient.SendMailAsync(message).ConfigureAwait(false);
        }
    }
}