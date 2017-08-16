// <copyright file="LeeftSamenBackgroundService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService
{
    using System;
    using System.Reflection;
    using System.ServiceProcess;

    using log4net;

    using Quartz;

    public class LeeftSamenBackgroundService : ServiceBase
    {
        private readonly ILog logger;

        private readonly IScheduler scheduler;

        public LeeftSamenBackgroundService(IScheduler scheduler)
        {
            this.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this.scheduler = scheduler;
        }

        public void Start()
        {
            this.logger.Info("Service starting");

            try
            {
                // Initiate the scheduler factory and start the scheduler
                this.scheduler.Start();
            }
            catch (Exception ex)
            {
                this.logger.Error("Error starting service", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            this.Start();
        }

        protected override void OnStop()
        {
            this.logger.Info("Stopping Service");

            try
            {
                this.scheduler.Shutdown(true);
            }
            catch (Exception ex)
            {
                this.logger.Error("Error stopping service", ex);
            }
        }
    }
}
