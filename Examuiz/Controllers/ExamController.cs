using AI_Layer.Interfaces;
using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost(Name = "create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExam(ExamDTOs.CreateExamDTO createExamDTO)
        {

            string? ContentAsHTML = await clsExam.CreateExamPrompt(createExamDTO, _generativeAI);
            if (string.IsNullOrWhiteSpace(ContentAsHTML))
                return BadRequest("Invalid PDF file");

            int startIndex = ContentAsHTML.IndexOf("```") + 8;
            int endIndex = ContentAsHTML.LastIndexOf("```");
            return CreatedAtRoute("create", ContentAsHTML.AsSpan(startIndex, endIndex - startIndex).ToString());
        }
    }
}
