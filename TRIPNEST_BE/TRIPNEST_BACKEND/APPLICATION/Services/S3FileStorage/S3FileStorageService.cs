using APPLICATION.DTOs.FileService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.S3FileStorage
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public S3FileStorageService()
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            var fName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var folderPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            var path = Path.Combine(folderPath, fName);

            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            // return local url (for production change to S3/Cloudinary url)
            var url = $"/uploads/{folder}/{fName}";
            return url;
        }
    }
}
