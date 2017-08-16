// <copyright file="IReportAbuseService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface IReportAbuseService
    {
        Task ReportAbuseAsync(User user, string reportDescription, string forUrl);
    }
}