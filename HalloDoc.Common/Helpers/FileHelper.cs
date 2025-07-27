using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HalloDoc.Common.Helpers
{
    public static class FileHelper
    {
        public static async Task<List<string>> SaveRequestFilesAsync(int requestId, List<IFormFile> files)
        {
            var savedPaths = new List<string>();
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", requestId.ToString());
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            foreach (var file in files)
            {
                var fileName = file.FileName;
                var filePath = Path.Combine(basePath, fileName);
                int version = 1;
                // If file exists, add version number
                while (File.Exists(filePath))
                {
                    var name = Path.GetFileNameWithoutExtension(fileName);
                    var ext = Path.GetExtension(fileName);
                    fileName = $"{name}_v{version}{ext}";
                    filePath = Path.Combine(basePath, fileName);
                    version++;
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Return relative path
                savedPaths.Add(Path.Combine("Documents", requestId.ToString(), fileName));
            }
            return savedPaths;
        }
    }
} 