// <copyright file="StringExtension.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LeeftSamen.Common.Extensions
{
    /// <summary>
    /// The string extension.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// The truncate.
        /// </summary>MO
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="maxLength">
        /// The max length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Truncate(this string text, int maxLength = 140)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
        }

        public static string TruncateKeepHtml(this string text, int maxLength = 140)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            {
                return text;
            }

            maxLength -= 3;
            var matches = Regex.Matches(text, "<[^>]*(>|$)");
            var sb = new StringBuilder();
            foreach (Match match in matches)
            {
                if (match.Index < maxLength)
                {
                    maxLength += match.Length;
                }
                else
                {
                    sb.Append(text.Substring(match.Index, match.Length));
                }
            }

            if (maxLength >= text.Length - 3)
            {
                return text;
            }

            sb.Insert(0, text.Substring(0, maxLength));
            sb.Append("...");
            return sb.ToString();
        }

        /// <summary>
        /// Strips all html from a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns the string without any html tags.</returns>
        public static string StripHtml(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = text.StripHtmlComments();
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(text, pattern, string.Empty);
        }

        public static string StripHtmlAndNormalize(this string input)
        {
            input = input.StripHtmlComments();
            return !string.IsNullOrEmpty(input)
                ? WebUtility.HtmlDecode(Regex.Replace(Regex.Replace(input, "<[^>]*(>|$)", string.Empty), "[\\s\r\n]+", " ")).Trim()
                : input;
        }

        private static string StripHtmlComments(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string pattern = @"(<!--[\S\s]+?(?=-->)(-->|\z))";
            return Regex.Replace(text, pattern, string.Empty);
        }
    }
}