// <copyright file="Program.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService
{
    using System.Reflection;

    public class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            JobServiceRunner.CreateAndRun(Assembly.GetExecutingAssembly(), (args.Length > 0) && (args[0] == "/console"));
        }
    }
}
