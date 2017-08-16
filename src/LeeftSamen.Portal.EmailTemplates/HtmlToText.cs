// <copyright file="HtmlToText.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using HtmlAgilityPack;

    /// <summary>
    /// The html to text.
    /// <see href="http://stackoverflow.com/a/25178738/190139">$Stackoverflow$</see>
    /// </summary>
    public static class HtmlToText
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Convert(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);
            return ConvertDoc(doc);
        }

        /// <summary>
        /// The convert doc.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ConvertDoc(HtmlDocument doc)
        {
            using (var sw = new StringWriter())
            {
                ConvertTo(doc.DocumentNode, sw);
                sw.Flush();
                return sw.ToString();
            }
        }

        /// <summary>
        /// The convert html.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ConvertHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return ConvertDoc(doc);
        }

        /// <summary>
        /// The convert to.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="outText">
        /// The out text.
        /// </param>
        public static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            ConvertTo(node, outText, new PreceedingDomTextInfo(false));
        }

        /// <summary>
        /// The convert content to.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="outText">
        /// The out text.
        /// </param>
        /// <param name="textInfo">
        /// The text info.
        /// </param>
        internal static void ConvertContentTo(HtmlNode node, TextWriter outText, PreceedingDomTextInfo textInfo)
        {
            foreach (var subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText, textInfo);
            }
        }

        /// <summary>
        /// The convert to.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="outText">
        /// The out text.
        /// </param>
        /// <param name="textInfo">
        /// The text info.
        /// </param>
        internal static void ConvertTo(HtmlNode node, TextWriter outText, PreceedingDomTextInfo textInfo)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:

                    // don't output comments
                    break;
                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText, textInfo);
                    break;
                case HtmlNodeType.Text:

                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                    {
                        break;
                    }

                    // get text
                    var html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                    {
                        break;
                    }

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Length == 0)
                    {
                        break;
                    }

                    if (!textInfo.WritePrecedingWhiteSpace || textInfo.LastCharWasSpace)
                    {
                        html = html.TrimStart();
                        if (html.Length == 0)
                        {
                            break;
                        }

                        textInfo.IsFirstTextOfDocWritten.Value = textInfo.WritePrecedingWhiteSpace = true;
                    }

                    outText.Write(HtmlEntity.DeEntitize(Regex.Replace(html.TrimEnd(), @"\s{2,}", " ")));
                    if (textInfo.LastCharWasSpace == char.IsWhiteSpace(html[html.Length - 1]))
                    {
                        outText.Write(' ');
                    }

                    break;
                case HtmlNodeType.Element:
                    string endElementString = null;
                    bool isInline;
                    var skip = false;
                    var listIndex = 0;
                    switch (node.Name)
                    {
                        case "nav":
                            skip = true;
                            isInline = false;
                            break;
                        case "body":
                        case "section":
                        case "article":
                        case "aside":
                        case "h1":
                        case "h2":
                        case "header":
                        case "footer":
                        case "address":
                        case "main":
                        case "div":
                        case "p": // stylistic - adjust as you tend to use
                            if (textInfo.IsFirstTextOfDocWritten)
                            {
                                outText.Write("\r\n");
                            }

                            endElementString = "\r\n";
                            isInline = false;
                            break;
                        case "br":
                            outText.Write("\r\n");
                            skip = true;
                            textInfo.WritePrecedingWhiteSpace = false;
                            isInline = true;
                            break;
                        case "a":
                            if (node.Attributes.Contains("href"))
                            {
                                var href = node.Attributes["href"].Value.Trim();
                                if (node.InnerText.IndexOf(href, StringComparison.InvariantCultureIgnoreCase) == -1)
                                {
                                    endElementString = "<" + href + ">";
                                }
                            }

                            isInline = true;
                            break;
                        case "li":
                            if (textInfo.ListIndex > 0)
                            {
                                outText.Write("\r\n{0}.\t", textInfo.ListIndex++);
                            }
                            else
                            {
                                outText.Write("\r\n*\t");

                                // using '*' as bullet char, with tab after, but whatever you want eg "\t->", if utf-8 0x2022
                            }

                            isInline = false;
                            break;
                        case "ol":
                            listIndex = 1;
                            goto case "ul";
                        case "ul":

                            // not handling nested lists any differently at this stage - that is getting close to rendering problems
                            endElementString = "\r\n";
                            isInline = false;
                            break;
                        case "img": // inline-block in reality
                            if (node.Attributes.Contains("alt"))
                            {
                                outText.Write('[' + node.Attributes["alt"].Value);
                                endElementString = "]";
                            }

                            if (node.Attributes.Contains("src"))
                            {
                                outText.Write('<' + node.Attributes["src"].Value + '>');
                            }

                            isInline = true;
                            break;
                        default:
                            isInline = true;
                            break;
                    }

                    if (!skip && node.HasChildNodes)
                    {
                        var info = isInline
                                       ? textInfo
                                       : new PreceedingDomTextInfo(textInfo.IsFirstTextOfDocWritten)
                                             {
                                                 ListIndex =
                                                     listIndex
                                             };
                        ConvertContentTo(node, outText, info);
                    }

                    if (endElementString != null)
                    {
                        outText.Write(endElementString);
                    }

                    break;
            }
        }

        /// <summary>
        /// The $bool$ wrapper.
        /// </summary>
        internal class BoolWrapper
        {
            /// <summary>
            /// Gets or sets a value indicating whether value.
            /// </summary>
            public bool Value { get; set; }

            /// <summary>
            /// The op_ implicit.
            /// </summary>
            /// <param name="boolWrapper">
            /// The $bool$ wrapper.
            /// </param>
            public static implicit operator bool(BoolWrapper boolWrapper)
            {
                return boolWrapper.Value;
            }

            /// <summary>
            /// The op_ implicit.
            /// </summary>
            /// <param name="boolWrapper">
            /// The $bool$ wrapper.
            /// </param>
            /// <returns>
            /// </returns>
            public static implicit operator BoolWrapper(bool boolWrapper)
            {
                return new BoolWrapper { Value = boolWrapper };
            }
        }

        /// <summary>
        /// The $preceeding$ dom text info.
        /// </summary>
        internal class PreceedingDomTextInfo
        {
            /// <summary>
            /// The is first text of doc written.
            /// </summary>
            public readonly BoolWrapper IsFirstTextOfDocWritten;

            /// <summary>
            /// Initializes a new instance of the <see cref="PreceedingDomTextInfo"/> class.
            /// </summary>
            /// <param name="isFirstTextOfDocWritten">
            /// The is first text of doc written.
            /// </param>
            public PreceedingDomTextInfo(BoolWrapper isFirstTextOfDocWritten)
            {
                this.IsFirstTextOfDocWritten = isFirstTextOfDocWritten;
            }

            /// <summary>
            /// Gets or sets a value indicating whether last char was space.
            /// </summary>
            public bool LastCharWasSpace { get; set; }

            /// <summary>
            /// Gets or sets the list index.
            /// </summary>
            public int ListIndex { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether write preceding white space.
            /// </summary>
            public bool WritePrecedingWhiteSpace { get; set; }
        }
    }
}