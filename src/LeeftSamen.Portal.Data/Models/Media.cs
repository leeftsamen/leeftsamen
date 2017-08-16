// <copyright file="Media.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class Media
    {
        public virtual DateTime CreationDate { get; set; }

        public virtual byte[] Data { get; set; }

        public virtual int MediaId { get; set; }

        public virtual string MimeType { get; set; }

        public virtual string Name { get; set; }

        public virtual int Size { get; set; }

        public string FontAwesomeClass()
        {
            var lowerName = this.Name.ToLower();
            if (lowerName.EndsWith(".pdf"))
            {
                return "fa-file-pdf-o";
            }
            else if (lowerName.EndsWith(".xls") || lowerName.EndsWith(".xlsx") || lowerName.EndsWith(".ods"))
            {
                return "fa-file-excel-o";
            }
            else if (lowerName.EndsWith(".doc") || lowerName.EndsWith(".docx") || lowerName.EndsWith(".odt"))
            {
                return "fa-file-word-o";
            }
            else if (lowerName.EndsWith(".mp4") || lowerName.EndsWith(".mov") || lowerName.EndsWith(".avi") || lowerName.EndsWith(".wmv"))
            {
                return " fa-file-video-o";
            }

            return string.Empty;
        }
    }
}