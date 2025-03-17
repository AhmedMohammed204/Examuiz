using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public static class Prompts
    {
        public static string CreateExamPrompt(ExamDTOs.CreateExamDTO createExamDTO)
        {
            return $@"
From this data I provided, create an exam of {createExamDTO.NumberOfQuestions} from {createExamDTO.FromPage} to {createExamDTO.ToPage} with {createExamDTO.QuestionTypes} questions of {createExamDTO.Difficulty} difficulty. return the as HTML tags (not HTML page, only tags so I can design them by myself).
No need to write introduction or explain anything, just the questions and answers.
IMPORTANT RULES TO FOLLOW:
- User different HTML Elements for questions.
- You can use Tables as well to provided specific questions types as needed. 
- Add some space between questions to allow answers within it. You can use border on Open ended questions to tell the student where he can write his answer (just border without any text within it).
- Don't write answers under questions, you can write the answers together as table in the last part of exam.
- Write the answers as with 2 columns, first column is the question number and the second column is the answer.
- Write questions in correct order
                    ";
        }
    }
}
