using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extentions
{
    public static class clsIFormFileExtentions
    {
        /// <summary>
        /// Check if the file is correct or not based on its extention and whether it is an empty file or not
        /// </summary>
        /// <param name="file"></param>
        /// <param name="Extension">the extension without "."</param>
        /// <returns></returns>
        public static bool IsCorrectFile(this IFormFile file, string Extension)
        {
            if (file == null || file.Length == 0) return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != $".{Extension}") return false;
            //if (file.ContentType != $"application/{Extension}") return false;

            return true;
        }

        public static bool IsFileSizeSafe(this IFormFile file, long LimitInMegaBytes)
        {
            return file.Length > LimitInMegaBytes;
        }
    }
}
