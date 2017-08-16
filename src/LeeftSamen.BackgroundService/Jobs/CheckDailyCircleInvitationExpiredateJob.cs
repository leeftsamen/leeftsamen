// <copyright file="CheckDailyCircleInvitationExpiredateJob.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.BackgroundService.Jobs
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;

    using LeeftSamen.Portal.EmailTemplates.Models;
    using LeeftSamen.Portal.Services;

    using log4net;

    using LeeftSamen.Common.InterfaceText;

    using Quartz;

    [DisallowConcurrentExecution]
    class CheckDailyCircleInvitationExpiredateJob : JobBase
    {
        private readonly ILog logger;
        private readonly IUserService userService;
        private readonly ITimelineService timelineService;
        private readonly IMailerService mailerService;

        private readonly ICircleService circleService;

        public CheckDailyCircleInvitationExpiredateJob(ILog logger, IUserService userService, ITimelineService timelineService, IMailerService mailerService, ICircleService circleService) : base(logger)
        {
            this.logger = logger;
            this.userService = userService;
            this.timelineService = timelineService;
            this.mailerService = mailerService;
            this.circleService = circleService;
        }

        public override async void Execute(IJobExecutionContext context)
        {
            this.logger.Info("CheckDailyCircleInvitationExpiredateJob.Execute");
            var users = await this.userService.GetAllUsersAsync();
            foreach (var user in users)
            {
                // Get al the invitations that has been send to the user
                foreach (var invitation in user.CircleInvitations)
                {
                    try
                    {
                        var aboutToExpireDate = invitation.ExpireDate.AddDays(-2); // date when to send the warning message(has to be -2 but -10 to test)
                        if (aboutToExpireDate.Date == DateTime.Now.Date)
                        {
                            // Send the email to the user
                            await this.mailerService.SendAsync(new CircleWarningExpiredateModel
                            {
                                CircleName = invitation.Circle.Name,
                                Name = user.Name,
                                ExpireDate = invitation.ExpireDate,
                                Subject = Subject.CircleInvitationWarningExpiredate,
                                PortalUrl = this.PortalUrl,
                                AcceptInvitationUrl = String.Format("{0}circles/{1}/acceptinvitation?code={2}", ConfigurationManager.AppSettings["PortalUrl"], invitation.Circle.CircleId, invitation.AcceptToken)
                            },
                            user.Email).ConfigureAwait(false);
                        }
                        else if (invitation.ExpireDate < DateTime.Now) // The invitation has expired
                        {
                            await this.circleService.RemoveInvitationAsync(invitation.Circle.CircleId, user.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.Info(e);
                    }
                }
            }
        }
    }
}
