// <copyright file="SuggestionService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Web.Configuration;

namespace LeeftSamen.Portal.Services
{
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;

    public class SuggestionService : ISuggestionService
    {
        private readonly IMailerService _mailerService;

        public SuggestionService(IMailerService mailerService)
        {
            this._mailerService = mailerService;
        }

        public async Task SendSuggestion(User user, string subject, string suggestion)
        {
            var model = new SuggestionModel
            {
                SendByName = user.Name,
                SendByEmail = user.Email,
                SuggestionSubject = subject,
                Suggestion = suggestion,
                Subject = string.Format(Subject.Suggestion, user.Name)
            };

            await this._mailerService.SendAsync(model, WebConfigurationManager.AppSettings["EmailSuggestions"]).ConfigureAwait(false);
        }
    }
}
