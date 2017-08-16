// <copyright file="JobFactory.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService.Utils
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    using LightInject;

    using log4net;

    using Quartz;
    using Quartz.Spi;

    public class JobFactory : IJobFactory
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IServiceContainer container;

        private readonly ConcurrentDictionary<Guid, Scope> scopeDictionary;

        public JobFactory(IServiceContainer container)
        {
            this.scopeDictionary = new ConcurrentDictionary<Guid, Scope>();
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            Logger.DebugFormat("Start new job: {0}", bundle.JobDetail.Key);

            var scopeId = Guid.NewGuid();
            var scope = this.container.BeginScope();

            this.scopeDictionary.GetOrAdd(scopeId, scope);

            var job = this.container.GetInstance(bundle.JobDetail.JobType) as JobBase;
            if (job != null)
            {
                job.ScopeId = scopeId;
            }
            else
            {
                this.scopeDictionary.TryRemove(scopeId, out scope);
                scope.Dispose();
            }

            return job;
        }

        public void ReturnJob(IJob job)
        {
            Logger.Debug("Return job");

            var baseJob = job as JobBase;

            if (baseJob != null && this.scopeDictionary.ContainsKey(baseJob.ScopeId))
            {
                Scope scope;
                this.scopeDictionary.TryRemove(baseJob.ScopeId, out scope);
                //scope.Completed += (sender, args) =>
                //    {
                //        scope.Dispose();
                //    };

                //if (scope != null)
                //{
                //    scope.Dispose();
                //}
            }
        }
    }
}