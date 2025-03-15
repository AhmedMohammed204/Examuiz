using AI_Layer.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
namespace Examuiz.Services
{
    public static class PdfService
    {
        public static (string text, List<string> images) ExtractTextAndImages(IFormFile file)
        {

            using var reader = new PdfReader(file.OpenReadStream());
            StringBuilder text = new StringBuilder();
            List<string> images = new List<string>();

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                PdfDictionary pageDict = reader.GetPageN(i);
                PdfDictionary resources = pageDict.GetAsDict(PdfName.RESOURCES);
                if (resources == null)
                    continue;
                PdfDictionary xobject = resources.GetAsDict(PdfName.XOBJECT);
                if (xobject == null)
                    continue;
                foreach (PdfName name in xobject.Keys)
                {
                    PdfObject obj = xobject.Get(name);
                    if (!obj.IsIndirect())
                        continue;
                    PdfDictionary tg = PdfReader.GetPdfObject(obj) as PdfDictionary;
                    if (tg == null)
                        continue;
                    PdfName subtype = tg.GetAsName(PdfName.SUBTYPE);
                    if (!PdfName.IMAGE.Equals(subtype))
                        continue;
                    try
                    {
                        PRStream prStream = (PRStream)tg;
                        var pdfImage = new iTextSharp.text.pdf.parser.PdfImageObject(prStream);
                        byte[] imageBytes = pdfImage.GetImageAsBytes();
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            images.Add(Convert.ToBase64String(imageBytes));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\n\nError: " + ex);
                    }
                }
            }

            return (text.ToString(), images);
        }
        public static int? GetPdfPageCount(IFormFile file)
        {
            try
            {
                using (var reader = new PdfReader(file.OpenReadStream()))
                {
                    return reader.NumberOfPages;
                }
            }
            catch { return null; }
        }

    }
}
