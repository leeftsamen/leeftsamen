// <copyright file="JobServiceRunner.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService
{
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;

    using LeeftSamen.BackgroundService.Utils;

    using LightInject;

    using Quartz;
    using Quartz.Impl;

    public class JobServiceRunner
    {
        public static void CreateAndRun(Assembly assembly, bool startWithoutServiceBase = false)
        {
            // Configure the service container with all IJob types from the calling assembly
            var container = LightInjectConfig.PreConfigureContainer();

            foreach (var jobType in assembly.GetTypes().Where(t => typeof(IJob).IsAssignableFrom(t)))
            {
                container.Register(jobType, new PerRequestLifeTime());
            }

            // Create a new scheduler and use the service container to resolve jobs.
            var scheduler = new StdSchedulerFactory().GetScheduler();
            scheduler.JobFactory = new JobFactory(container);

            var backgroundService = new LeeftSamenBackgroundService(scheduler);

            // Initialize the service and start it.
            if (startWithoutServiceBase)
            {
                backgroundService.Start();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { backgroundService });
            }
        }
    }
}