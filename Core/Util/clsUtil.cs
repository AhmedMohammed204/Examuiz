using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DTOs.ExamDTOs;

namespace Core.Util
{
    public static class clsUtil
    {
        public static bool IsFileExtension(IFormFile file, string Extension)
        {
            if (file == null || file.Length == 0) return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != $".{Extension}") return false;
            if (file.ContentType != $"application/{Extension}") return false;

            return true;
        }
    }
}
