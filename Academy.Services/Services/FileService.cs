using Academy.Core.ServicesInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services
{
    public class FileService(IWebHostEnvironment environment) : IFileService 
    {
       

        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "Uploads");
            // path = "c://projects/productminiapi/uploads", not exactly, but something like that

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
           var ext = Path.GetExtension(imageFile.FileName);
          if (!allowedFileExtensions.Contains(ext))
            {
                throw new ArgumentException($"Only{string.Join(",", allowedFileExtensions)} are allowed.");
            }
            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync (stream);
            return fileName;
            // باقي الكود الخاص بالتحقق من الامتداد وحفظ الملف ممكن يكون تحت هذا السطر
        }

        public void DeleteFile(string fileNameWithExtension)
        {
            if(string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentException(nameof(fileNameWithExtension));
            }

            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

            if(!File.Exists(path))
            {
                throw new FileNotFoundException($"Invalied file path");

            }
            File.Delete(path);
        }


    }
}
