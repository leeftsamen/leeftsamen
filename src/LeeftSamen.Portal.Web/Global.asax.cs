// <copyright file="Global.asax.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Configuration;
    using LeeftSamen.Portal.Services;

    using Mindscape.Raygun4Net;
    using log4net;

    public class MvcApplication : HttpApplication, IRaygunApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUserService userService;

        public static string SessionCookieName
        {
            get
            {
                return ((SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState")).CookieName;
            }
        }

        public static void DeleteSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            DeleteCookie(SessionCookieName);
        }

        public RaygunClient GenerateRaygunClient()
        {
            var client = new RaygunClient();
            client.IgnoreFormFieldNames("password,confirmpassword,newpassword");
            client.ApplicationVersion =
                FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            if (HttpContext.Current.User != null)
            {
                client.User = HttpContext.Current.User.Identity.Name;
            }

            return client;
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_BeginRequest()
        {
            // Try to clear and remove the Server header
            this.Response.Headers["Server"] = string.Empty;
            this.Response.Headers.Remove("Server");
        }

        protected void Application_Error()
        {
            var allErrors = this.Context.AllErrors;
            if (allErrors != null && (allErrors.Length == 1 && allErrors[0].GetType() == typeof(HttpException)))
            {
                var err = (HttpException)allErrors[0];
                Log.Fatal("Strange error:");
                Log.Fatal(err);
            }

            var lastError = this.Server.GetLastError();
            Log.Fatal("Last error:");
            Log.Fatal(lastError.Message);

            try
            {
                Log.Fatal(this.Request.Url.PathAndQuery);
                if (allErrors != null)
                {
                    foreach (var er in allErrors)
                    {
                        Log.Fatal(er);
                    }
                }
            }
            catch (Exception er)
            {
                Log.Fatal(er);
            }

            // Don't mess with ASP.Net errors in debug mode.
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                return;
            }

            // Hack to reset the ASP.Net error page for uncaught exceptions (404, 403, 500, etc)
            var exception = this.Server.GetLastError() as HttpException;
            this.Server.ClearError();
            this.Response.StatusCode = exception != null
                                           ? exception.GetHttpCode()
                                           : (int)HttpStatusCode.InternalServerError;
        }

        private static void DeleteCookie(string name)
        {
            var cookie = new HttpCookie(name, string.Empty)
                             {
                                 Expires =
                                     new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                             };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}