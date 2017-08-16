// <copyright file="AtomResult.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Helpers
{
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using System.Xml;

    public class AtomResult : ActionResult
    {
        private readonly SyndicationFeed feed;

        public AtomResult(SyndicationFeed feed)
        {
            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/atom+xml";

            var formatter = new Atom10FeedFormatter(this.feed);
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}