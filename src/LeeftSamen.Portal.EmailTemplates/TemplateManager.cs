// <copyright file="TemplateManager.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates
{
    using System;
    using System.IO;

    using RazorEngine.Templating;

    /// <summary>
    /// The template resolver.
    /// </summary>
    public class TemplateManager : ITemplateManager
    {
        public TemplateManager()
        {
        }

        /// <summary>
        /// The add dynamic.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotImplementedException("dynamic templates are not supported!");
        }

        /// <summary>
        /// The get key.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="resolveType">
        /// The resolve type.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ITemplateKey"/>.
        /// </returns>
        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new NameOnlyTemplateKey(name, resolveType, context);
        }

        /// <summary>
        /// The resolve.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ITemplateSource"/>.
        /// </returns>
        public ITemplateSource Resolve(ITemplateKey key)
        {
            var templateFile = string.Format(
                AppDomain.CurrentDomain.RelativeSearchPath + @"\Templates\{0}.cshtml",
                key.Name);

            if (!File.Exists(templateFile))
            {
                templateFile = string.Format(AppDomain.CurrentDomain.BaseDirectory + @"Templates\{0}.cshtml", key.Name);

                if (!File.Exists(templateFile))
                {
                    throw new Exception(string.Format("Template file with name: {0} not exists", templateFile));
                }
            }

            var templateStream = new StreamReader(templateFile);

            return new LoadedTemplateSource(templateStream.ReadToEnd());
        }
    }
}