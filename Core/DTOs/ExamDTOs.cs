using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class ExamDTOs
    {
        public class CreateExamDTO
        {
            
            public required byte NumberOfQuestions { get; set; }
            public required ushort FromPage { get; set; }
            public required ushort ToPage { get; set; }
            public required string QuestionTypes { get; set; }
            public required string Difficulty { get; set; }
            public required IFormFile ExamTextBook    { get; set; }
        }
        public class CorrectingExamScoresDTO
        {
            public required IFormFile StudentsAnswers { get; set; }
            public required string SubjectName { get; set; }
        }
        public class AnalyzeExamAnswersDTO
        {
            public required IFormFile StudentsAnswersExcelFile { get; set; }
            public required IFormFile ExamPDF_File { get; set; }
            public required string SubjectName { get; set; }
        }


    }
}
