// <copyright file="ReportAbuseService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;

    public class ReportAbuseService : IReportAbuseService
    {
        private readonly IMailerService mailerService;

        public ReportAbuseService(IMailerService mailerService)
        {
            this.mailerService = mailerService;
        }

        public async Task ReportAbuseAsync(User user, string reportDescription, string forUrl)
        {
            var model = new ReportAbuseModel
            {
                ReportDescription = reportDescription,
                SendByName = user.Name,
                SendByEmail = user.Email,
                ForUrl = forUrl,
                Subject = string.Format(Subject.ReportAbuse, user.Name)
            };

            await this.mailerService.SendAsync(model, "info@leeftsamen.nl").ConfigureAwait(false);
        }
    }
}