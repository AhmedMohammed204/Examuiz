// Core/Services/PdfService.cs
using Core.Extentions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Services
{
    public static class PdfService
    {
        public static string ExtractText(IFormFile file)
        {
            if (file.IsFileSizeSafe( 10 * 1024 * 1024))
                throw new ArgumentException("File size exceeds 10MB limit");


            using var reader = new PdfReader(file.OpenReadStream());
            StringBuilder text = new StringBuilder();

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            }

            return text.ToString();
        }

        public static List<string> ExtractImages(IFormFile file)
        {
            if (file.IsFileSizeSafe(10 * 1024 * 1024))
                throw new ArgumentException("File size exceeds 10MB limit");

            List<string> images = new List<string>();
            using var reader = new PdfReader(file.OpenReadStream());


            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfDictionary pageDict = reader.GetPageN(i);
                PdfDictionary resources = pageDict.GetAsDict(PdfName.RESOURCES);
                if (resources == null) continue;

                PdfDictionary xobject = resources.GetAsDict(PdfName.XOBJECT);
                if (xobject == null) continue;

                foreach (PdfName name in xobject.Keys)
                {
                    PdfObject obj = xobject.Get(name);
                    if (!obj.IsIndirect()) continue;

                    PdfDictionary tg = PdfReader.GetPdfObject(obj) as PdfDictionary;
                    if (tg == null) continue;

                    if (!PdfName.IMAGE.Equals(tg.GetAsName(PdfName.SUBTYPE))) continue;

                    try
                    {
                        PRStream prStream = (PRStream)tg;
                        var pdfImage = new PdfImageObject(prStream);
                        byte[] imageBytes = pdfImage.GetImageAsBytes();

                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            images.Add(Convert.ToBase64String(imageBytes));
                        }
                    }
                    catch { /* Handle exception */ }
                }
            }
            return images;
        }
        public static PdfData ExtractPdfData(IFormFile file)
        {
            return new PdfData(ExtractText(file), ExtractImages(file));
        }

        public static int? GetPdfPageCount(IFormFile file)
        {
            try
            {
                using var reader = new PdfReader(file.OpenReadStream());
                return reader.NumberOfPages;
            }
            catch { return null; }
        }
    }
}