// <copyright file="HttpPostedFileBaseExtensions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Extensions
{
    using System.Web;

    public static class HttpPostedFileBaseExtensions
    {
        public static bool IsImage(this HttpPostedFileBase postedFile)
        {
            return IsImage(postedFile, false);
        }

        public static bool IsImage(this HttpPostedFileBase postedFile, bool allowNull)
        {
            if (allowNull && postedFile == null)
            {
                return true;
            }

            return postedFile != null && !string.IsNullOrWhiteSpace(postedFile.ContentType)
                   && IsImageContentType(postedFile.ContentType);
        }

        public static bool IsImageContentType(string contentType)
        {
            return contentType == "image/jpg" || contentType == "image/jpeg" || contentType == "image/pjpeg"
                   || contentType == "image/gif" || contentType == "image/x-png" || contentType == "image/png";
        }
    }
}