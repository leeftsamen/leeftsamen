// <copyright file="MediaService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Web;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Data.Entity;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;

    public class MediaService : IMediaService
    {
        private readonly IApplicationDbContext databaseContext;

        public MediaService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Media CreateMedia(ImageDto image)
        {
            var imageData = GetBytes(image);
            var media = this.databaseContext.Media.Create();
            media.Name = image.FileName;
            media.Size = imageData.Length;
            media.MimeType = GetMimeType(image.Format);
            media.Data = imageData;
            media.CreationDate = DateTime.Now;

            return media;
        }

        public Media CreateMedia(HttpPostedFileBase file)
        {
            var imageData = GetBytes(file);
            var media = this.databaseContext.Media.Create();
            media.Name = file.FileName;
            media.Size = imageData.Length;
            media.MimeType = GetMimeType(file);
            media.Data = imageData;
            media.CreationDate = DateTime.Now;

            return media;
        }

        public async Task DeleteAsync(Media media)
        {
            this.databaseContext.Media.Remove(media);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Media> GetByIdAsync(int mediaId)
        {
            return await this.databaseContext.Media.FirstOrDefaultAsync(m => m.MediaId == mediaId).ConfigureAwait(false);
        }

        public async Task<Media> InsertAsync(string name, int size, string mimeType, byte[] data)
        {
            var media = this.databaseContext.Media.Create();
            media.Name = name;
            media.Size = data.Length;
            media.MimeType = mimeType;
            media.Data = data;
            media.CreationDate = DateTime.Now;

            this.databaseContext.Media.Add(media);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return media;
        }

        public async Task ReplaceAsync(Media media, string name, int size, string mimeType, byte[] data)
        {
            media.Name = name;
            media.Size = size;
            media.MimeType = mimeType;
            media.Data = data;
            media.CreationDate = DateTime.Now;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private static byte[] GetBytes(ImageDto image)
        {
            using (var stream = new MemoryStream())
            {
                if (image.Format.Guid == ImageFormat.Jpeg.Guid)
                {
                    var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                    var myEncoder = Encoder.Quality;

                    var myEncoderParameters = new EncoderParameters(1);
                    var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Image.Save(stream, jgpEncoder, myEncoderParameters);
                }
                else
                {
                    image.Image.Save(stream, image.Format);
                }

                return stream.ToArray();
            }
        }

        private static byte[] GetBytes(HttpPostedFileBase file)
        {
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }

            return fileData;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static string GetMimeType(ImageFormat imageFormat)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }

        private static readonly Dictionary<string, string> MimetypeDictionary = new Dictionary<string, string>
        {
            { "doc", "application/msword" },
            { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "xls", "application/excel" },
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "pdf", "application/pdf" },
            { "odt", "application/vnd.oasis.opendocument.text" },
            { "ods", "application/vnd.oasis.opendocument.spreadsheet" }
        };

        private static string GetMimeType(HttpPostedFileBase file)
        {
            string extension = file.FileName.Split('.').LastOrDefault();
            if (extension != null)
            {
                if (MimetypeDictionary.ContainsKey(extension.ToLower()))
                {
                    return MimetypeDictionary[extension.ToLower()];
                }
            }

            return file.ContentType;
        }
    }
}