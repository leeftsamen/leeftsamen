// <copyright file="MessageGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.EmailTemplates.Models;

    using RazorEngine.Templating;

    /// <summary>
    /// The message generator.
    /// </summary>
    public class MessageGenerator : IMessageGenerator
    {
        /// <summary>
        /// The razor engine service.
        /// </summary>
        private readonly IRazorEngineService razorEngineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageGenerator"/> class.
        /// </summary>
        /// <param name="razorEngineService">
        /// The razor Engine Service.
        /// </param>
        public MessageGenerator(IRazorEngineService razorEngineService)
        {
            this.razorEngineService = razorEngineService;
        }

        /// <summary>
        /// The generate message.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <typeparam name="T">
        /// The type of the model
        /// </typeparam>
        /// <returns>
        /// The <see cref="MailMessage"/>.
        /// </returns>
        public MailMessage GenerateMessage<T>(T model) where T : IEmailTemplateModel
        {
            var templateFile = string.Format(
                AppDomain.CurrentDomain.RelativeSearchPath + @"\Templates\{0}.cshtml",
                model.TemplateName);

            if (!File.Exists(templateFile))
            {
                templateFile = string.Format(
                    AppDomain.CurrentDomain.BaseDirectory + @"Templates\{0}.cshtml",
                    model.TemplateName);

                if (!File.Exists(templateFile))
                {
                    throw new Exception(string.Format("Template file with name: {0} not exists", templateFile));
                }
            }

            // Do not remove this dirty, dirty hack!
            // Because the Razor templates are compiled at runtime, and the Common reference is not used
            // anywhere else in the BackgroundSerivce, the assemly is never resolved unless we make the call below:
            var hack = Email.CircleInvitationAcceptLinkText;

            string htmlBody = string.Empty;
            try
            {
                htmlBody = this.razorEngineService.RunCompile(model.TemplateName, typeof(T), model);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            var textBody = HtmlToText.ConvertHtml(htmlBody);
            var message = new MailMessage { Subject = model.Subject, BodyTransferEncoding = TransferEncoding.Base64};
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(textBody, Encoding.UTF8, "text/plain"));

            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html");
            try
            {
                LinkedResource logo = null;
                if (model.Company == "comunios")
                {
                    logo = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + @"Content\Images\comunios_logo.png");
                }
                else
                {
                    logo = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + @"Content\Images\leeftsamen_logo.png");
                }

                logo.ContentId = "logo";
                htmlView.LinkedResources.Add(logo);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            message.AlternateViews.Add(htmlView);

            return message;
        }
    }
}