using AI_Layer.Interfaces;
using Core.DTOs;
using Core.Extentions;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using static Core.DTOs.ExamDTOs;
namespace Examuiz.Controllers
{
    [Route("api/exam")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        IGenerativeAI _generativeAI;
        public ExamController(IGenerativeAI generativeAI)
        {
            _generativeAI = generativeAI;
        }



        [HttpPost("create", Name = "createExam")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExam(ExamDTOs.CreateExamDTO createExamDTO)
        {

            string? ContentAsHTML = await clsExam.CreateExamPrompt(createExamDTO, _generativeAI);
            if (string.IsNullOrWhiteSpace(ContentAsHTML))
                return BadRequest("Invalid PDF file");

            int startIndex = ContentAsHTML.IndexOf("```") + 8;
            int endIndex = ContentAsHTML.LastIndexOf("```");
            return CreatedAtRoute("createExam", ContentAsHTML.AsSpan(startIndex, endIndex - startIndex).ToString());
        }
    







        [HttpPost("correct", Name ="correctAnswers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CorrectExamScores(ExamDTOs.CorrectingExamScoresDTO correctingExamScoresDTO)
        {
            string? ContentAsHTML = await clsExam.CorrectExamScores(correctingExamScoresDTO, _generativeAI);
            if (string.IsNullOrWhiteSpace(ContentAsHTML))
                return BadRequest("Invalid PDF file");
            if (!ContentAsHTML.Contains("```")) return BadRequest("Invalid PDF file");
            int startIndex = ContentAsHTML.IndexOf("```") + 8;
            int endIndex = ContentAsHTML.LastIndexOf("```");
            string res = ContentAsHTML.AsSpan(startIndex, endIndex - startIndex).ToString();
            if (res == "<p>not exam</p>\n")
                return BadRequest("Invalid PDF file");
            return CreatedAtRoute("correctAnswers", res);
        }

        [HttpPost("analyze", Name = "AnalyzingStudentsAnswers")]
        public async Task<IActionResult> AnalyzingStudentsAnswers(AnalyzeExamAnswersDTO analyzeExamAnswersDTO)
        {
            string? response = await clsExam.AnalyzingStudentsAnswers(analyzeExamAnswersDTO, _generativeAI);
            if (!response.Contains("```")) return BadRequest("Invalid PDF file");
            int startIndex = response.IndexOf("```") + 8;
            int endIndex = response.LastIndexOf("```");
            string result = response.AsSpan(startIndex, endIndex - startIndex).ToString();
            return Created("AnalyzingStudentsAnswers", result);

        }


    }
}
