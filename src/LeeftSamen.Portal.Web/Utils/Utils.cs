// <copyright file="Utils.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Utils
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    internal static class Utils
    {
        public static bool RequestedByApp(HttpRequestBase request)
        {
            var requestedByApp = false;
            if (request.UserAgent != null)
            {
                requestedByApp = request.UserAgent.StartsWith("LeeftSamen.App/");
            }

            return requestedByApp;
        }

        public static string GetAcceptTokenFromReturnUrl(string returnUrl)
        {
            var match = Regex.Match(returnUrl, @"code=([\w-]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        public static void SetReturnUrlCookie(string returnUrl, HttpRequestBase request, HttpResponseBase response)
        {
            var cookie = request.Cookies["returnUrl"];
            if (cookie == null)
            {
                cookie = new HttpCookie("returnUrl") { Value = returnUrl, Expires = DateTime.Now.AddDays(1) };
                response.Cookies.Add(cookie);
            }
            else
            {
                cookie.Value = returnUrl;
                response.SetCookie(cookie);
            }
        }

        public static string CheckReturnUrlCookie(HttpRequestBase request, HttpResponseBase response)
        {
            var cookie = request.Cookies["returnUrl"];
            if (cookie == null)
            {
                return null;
            }

            cookie.Expires = DateTime.Now.AddDays(-1d);
            response.Cookies.Add(cookie);

            return cookie.Value;
        }

        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return false;
            }

            try
            {
                // Regex from https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx

                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}