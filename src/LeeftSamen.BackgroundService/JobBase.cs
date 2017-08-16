// <copyright file="JobBase.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService
{
    using System;

    using log4net;

    using Quartz;
    using System.Threading.Tasks;

    public abstract class JobBase : IJob
    {
        protected readonly ILog Logger;
        protected readonly string PortalUrl;

        protected JobBase(ILog logger)
        {
            this.Logger = logger;
            this.PortalUrl = "https://app.leeftsamen.nl/";
        }

        public Guid ScopeId { get; set; }

        public abstract void Execute(IJobExecutionContext context);
    }
}