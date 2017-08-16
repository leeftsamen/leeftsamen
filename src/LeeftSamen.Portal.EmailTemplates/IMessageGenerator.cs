// <copyright file="IMessageGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates
{
    using System.Net.Mail;

    using LeeftSamen.Portal.EmailTemplates.Models;

    /// <summary>
    /// The MessageGenerator interface.
    /// </summary>
    public interface IMessageGenerator
    {
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
        MailMessage GenerateMessage<T>(T model) where T : IEmailTemplateModel;
    }
}