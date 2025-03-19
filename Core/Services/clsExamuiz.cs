using AI_Layer.Interfaces;
using Core.DTOs;
using Core.Extentions;
using static Core.DTOs.ExamDTOs;

namespace Core.Services
{
    public static class clsExam
    {

        private static bool _CheckPDF_Pages(ExamDTOs.CreateExamDTO createExamDTO)
        {
            int? PageCount = PdfService.GetPdfPageCount(createExamDTO.ExamTextBook);
            if (PageCount == null) return false;
            if (createExamDTO.FromPage > PageCount || createExamDTO.ToPage > PageCount) return false;
            return true;
        }
        private static async Task<string> _GeContentFromAI(IGenerativeAI generativeAI, string Prompt, IList<string>? images)
        {

            string PDF_Content = await generativeAI.TextGenerate(Prompt, images);

            return PDF_Content;
        }
        public static async Task<string?> CreateExamPrompt(ExamDTOs.CreateExamDTO createExamDTO, IGenerativeAI generativeAI)
        {
            if (!createExamDTO.ExamTextBook.IsCorrectFile("pdf")) return null;
            if (!_CheckPDF_Pages(createExamDTO)) return null;

            var pdfData = PdfService.ExtractPdfData(createExamDTO.ExamTextBook);

            if (string.IsNullOrWhiteSpace(pdfData.text) && pdfData.images.Count == 0)
                return null;

            string Prompt = Prompts.CreateExamPrompt(createExamDTO, pdfData.text);
            string ContentAsHTML = await _GeContentFromAI(generativeAI, Prompt, pdfData.images);

            return ContentAsHTML;

        }
        public static async Task<string?> CorrectExamScores(ExamDTOs.CorrectingExamScoresDTO correctingExamScoresDTO, IGenerativeAI generativeAI)
        {
            if (!correctingExamScoresDTO.StudentsAnswers.IsCorrectFile("pdf")) return null;

            var images = PdfService.ExtractImages(correctingExamScoresDTO.StudentsAnswers);

            if (images.Count == 0)
                throw new Exception("correct exam must be: PDF File and only images");

            string Prompt = Prompts.CorrectExam(correctingExamScoresDTO, null);
            string ContentAsHTML = await _GeContentFromAI(generativeAI, Prompt, images);
            return ContentAsHTML;


        }

        public static async Task<string?> AnalyzingStudentsAnswers(AnalyzeExamAnswersDTO analyzeExamAnswersDTO, IGenerativeAI generativeAI)
        {
            if (!analyzeExamAnswersDTO.ExamPDF_File.IsCorrectFile("pdf")) throw new Exception("Invalid Exam PDF File");
            var images = PdfService.ExtractImages(analyzeExamAnswersDTO.ExamPDF_File);

            if (images.Count == 0)
                throw new Exception("correct exam must be: PDF File and only images");
            List<string> data = await ExcelService.ReadExcelFile(analyzeExamAnswersDTO.StudentsAnswersExcelFile);
            string Prompt = Prompts.AnalyzingStudentsAnswers( data.JoinString()  , analyzeExamAnswersDTO.SubjectName );
            string ContentAsHTML = await _GeContentFromAI(generativeAI, Prompt, images);
            return ContentAsHTML;
        }
    }
}
