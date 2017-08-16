// <copyright file="LeeftSamenBackgroundServiceInstaller.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public class LeeftSamenBackgroundServiceInstaller : Installer
    {
        public LeeftSamenBackgroundServiceInstaller()
        {
            ServiceProcessInstaller process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };

            ServiceInstaller service = new ServiceInstaller
            {
                ServiceName = "LeeftSamen Scheduler",
                Description = "LeeftSamen Windows Service for automated tasks"
            };

            this.Installers.Add(process);
            this.Installers.Add(service);
        }
    }
}