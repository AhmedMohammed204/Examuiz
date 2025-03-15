using AI_Layer.Interfaces;
using Core.DTOs;
using Core.Util;

namespace Examuiz.Services
{
    public static class clsExam
    {
        public static async Task<string?> CreateExamPrompt( ExamDTOs.CreateExamDTO createExamDTO, IGenerativeAI generativeAI)
        {
            if(!clsUtil.IsFileExtension(createExamDTO.ExamTextBook, "pdf")) return null;
            if(!_CheckPDF_Pages(createExamDTO)) return null;

            var (pdfText, images) = PdfService.ExtractTextAndImages(createExamDTO.ExamTextBook);

            if (string.IsNullOrWhiteSpace(pdfText) && images.Count == 0)
                return null;

            string ContentAsHTML = await _GeContentFromAI(pdfText, images, generativeAI, Prompts.CreateExamPrompt(createExamDTO));
            return ContentAsHTML;

        }
        private static bool _CheckPDF_Pages(ExamDTOs.CreateExamDTO createExamDTO)
        {
            int? PageCount = PdfService.GetPdfPageCount(createExamDTO.ExamTextBook);
            if (PageCount == null) return false;
            if (createExamDTO.FromPage > PageCount || createExamDTO.ToPage > PageCount) return false;
            return true;
        }
        private static async Task<string> _GeContentFromAI(string text, List<string> images, IGenerativeAI generativeAI, string Prompt)
        {

            string PDF_Content = await generativeAI.FileGenerate(Prompt + text + images);

            return PDF_Content;
        }
    }
}
