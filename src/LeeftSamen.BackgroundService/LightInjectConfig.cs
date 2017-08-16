// <copyright file="LightInjectConfig.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService
{
    using System;
    using System.Data.Entity;
    using System.Net.Mail;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates;
    using LeeftSamen.Portal.Services;

    using LightInject;

    using log4net;

    using LeeftSamen.BackgroundService.Utils;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using RazorEngine.Configuration;
    using RazorEngine.Templating;

    internal class LightInjectConfig
    {
        public static IServiceContainer PreConfigureContainer()
        {
            var container = new ServiceContainer();

            // Register the Database context using the first connection string
            container.Register<IApplicationDbContext>(
                factory => new ApplicationDbContext("DefaultConnection"),
                new PerScopeLifetime());

            // Register all default implementations from the Data and Services assemblies.
            container.RegisterAssembly(typeof(IMailerService).Assembly, () => new PerScopeLifetime());

            container.Register<ILinkGenerator>(
                factory => new LinkGenerator(),
                new PerScopeLifetime());

            //container.Register(
            //    factory =>
            //    new ActivityService(
            //        container.GetInstance<IApplicationDbContext>(),
            //        null,
            //        container.GetInstance<INotificationService>()),
            //    new PerScopeLifetime());

            //container.Register(
            //    factory =>
            //    new CircleService(
            //        container.GetInstance<IApplicationDbContext>(),
            //        container.GetInstance<IMediaService>(),
            //        container.GetInstance<IMailerService>(),
            //        container.GetInstance<IRandomGenerator>(),
            //        null,
            //        container.GetInstance<INotificationService>()),
            //    new PerScopeLifetime());

            // The UserService is based on an Identity UserManager which needs custom configuration. Also make sure
            // the IUserService resolves to this configured instance.
            container.Register(factory => CreateAndConfigureUserService(container), new PerScopeLifetime());
            container.Register<IUserService>(factory => container.GetInstance<UserService>(), new PerScopeLifetime());

            // Register a SMTP client. Configuration is found in the web.config.
            container.Register<SmtpClient>(new PerScopeLifetime());

            // Register and configure the MessageGenerator with RazorEngine
            container.Register<IMessageGenerator>(
                factory =>
                new MessageGenerator(
                    RazorEngineService.Create(
                        new TemplateServiceConfiguration
                            {
                                TemplateManager = new TemplateManager()
                            })),
                new PerScopeLifetime());

            container.Register<IUserStore<User>>(
                factory => new UserStore<User>((DbContext)container.GetInstance<IApplicationDbContext>()),
                new PerScopeLifetime());

            container.RegisterConstructorDependency((factory, info) => LogManager.GetLogger(info.Member.DeclaringType));

            return container;
        }

        private static UserService CreateAndConfigureUserService(IServiceFactory container)
        {
            var manager = new UserService(
                container.GetInstance<IUserStore<User>>(),
                container.GetInstance<IApplicationDbContext>(),
                container.GetInstance<IMailerService>(),
                container.GetInstance<IMediaService>(),
                container.GetInstance<ILinkGenerator>());

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
                                        {
                                            AllowOnlyAlphanumericUserNames = false,
                                            RequireUniqueEmail = true
                                        };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
                                            {
                                                RequiredLength = 8,
                                                RequireNonLetterOrDigit = false,
                                                RequireDigit = false,
                                                RequireLowercase = false,
                                                RequireUppercase = false,
                                            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider(
                "Phone Code",
                new PhoneNumberTokenProvider<User> { MessageFormat = "Your security code is {0}" });
            manager.RegisterTwoFactorProvider(
                "Email Code",
                new EmailTokenProvider<User> { Subject = "Security Code", BodyFormat = "Your security code is {0}" });
            manager.EmailService = container.GetInstance<IMailerService>();

            return manager;
        }
    }
}