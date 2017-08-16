// <copyright file="IMediaService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Web;

namespace LeeftSamen.Portal.Services
{
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;

    public interface IMediaService
    {
        Media CreateMedia(ImageDto image);

        Media CreateMedia(HttpPostedFileBase image);

        Task DeleteAsync(Media media);

        Task<Media> GetByIdAsync(int mediaId);

        Task<Media> InsertAsync(string name, int size, string mimeType, byte[] data);

        Task ReplaceAsync(Media media, string name, int size, string mimeType, byte[] data);
    }
}