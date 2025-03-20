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
        public static string CreateExamPrompt(ExamDTOs.CreateExamDTO createExamDTO, string Text)
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
- Give answers section class='answers' to be able to find it 
- Write questions in correct order

The Data:
{Text}
                    ";
        }

        public static string CorrectExam(ExamDTOs.CorrectingExamScoresDTO correctingExamScoresDTO, string? Text)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine("INSTRUCTIONS");
            prompt.AppendLine($"From these images I provided, correct the exam of {correctingExamScoresDTO.SubjectName} and return the corrected exam as HTML tags (not HTML page, only tags so I can design them by myself).");
            prompt.AppendLine("No need to write introduction or explain anything, just the questions and answers.");
            prompt.AppendLine("The exam can be more than 1 page so you can map students according to thier names on the top of each page (image)");
            prompt.AppendLine("Students answers are written by blue pen so you can find it");
            prompt.AppendLine("Stucents answered MCQ by blue circle");
            prompt.AppendLine("The Exam is out of 20");
            prompt.AppendLine("Students answers are wrrten by plue so if you find another color don't consider it as student answer");
            prompt.AppendLine("If the student didn't answer the question, write it as '-'");
            prompt.AppendLine("Black color is question, and blue is students answers");
            prompt.AppendLine("IMPORTANT RULES TO FOLLOW:");
            prompt.AppendLine("Each student wrote his answer using blue pen");
            prompt.AppendLine("If the student didn't answer write 'Student didn't provide an answer' and mark him with 0");
            prompt.AppendLine("Students may skiped some questions so mark them as 0");
            prompt.AppendLine("Start write the HTML following instructions wthout writing an introduction or any explanation");
            //prompt.AppendLine("- If the data provided is not an exam, return null.");
            prompt.AppendLine("- Write the score of each student as the following format:");
            prompt.AppendLine("    - Student Name");
            prompt.AppendLine("    - Then table of 3 columns (Student answer, Correct answer, justification (3 to 4 sentences ) , Student Score) for each question.");
            prompt.AppendLine("    - At the end of the table, write the total score of the student.");
            prompt.AppendLine("    - Write them in descending order according to the score.");
            prompt.AppendLine("- After scoring each student, write the following:");
            prompt.AppendLine("    - Write the total number of students who took the exam.");
            prompt.AppendLine("    - Write the average score of the students.");
            prompt.AppendLine($"- Be accurate in scoring.");
            prompt.AppendLine("- At the end write inside tage <div class='analyzing'> the following:");
            prompt.AppendLine("<div class='analyzing'>");
            prompt.AppendLine("{");
            prompt.AppendLine("    'Questions' : ");
            prompt.AppendLine("    {");
            prompt.AppendLine("            /* each question number and total wrong answers on it + the percentage of the wrongs */");
            prompt.AppendLine("    },");
            prompt.AppendLine("    'Subjects':");
            prompt.AppendLine("    {");
            prompt.AppendLine("        'Hardest' : /* the hardest subject according the students answers*/,");
            prompt.AppendLine("        'Easiest' : /* the easiest subject according the students answers*/,");
            prompt.AppendLine("    },");
            prompt.AppendLine("    'Advice' : /* in 15 to 30 sentences write in deep advices to enhance the teachers skills and students socres (if they where bad). Also give some advice about hardest subjects to students (At the end of the day the most improtant thing is enhancing students level)  */");
            prompt.AppendLine("}");
            prompt.AppendLine("</div>");
            if (!string.IsNullOrWhiteSpace(Text))
            {

                prompt.AppendLine("The Exam data is:");
                prompt.AppendLine($"Text data: {Text}");
            }

            return prompt.ToString();

            //VERY IMPORTANT TO FOLLOW:
            //if the data provided is not an exam remove every text that you write and write <p>not exam</p> instead.
        }

        public static string AnalyzingStudentsAnswers(string StudentsData, string SubjectName, PdfData? examData)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine("Students Data: ");
            prompt.AppendLine(StudentsData);
            if(examData.text != null)
            {
                prompt.AppendLine("Exam Text: ");
                prompt.AppendLine(examData.text);

            }
            prompt.AppendLine($"\nThe above data is students grades in {SubjectName} exam.");
            if(examData.images != null)
                prompt.AppendLine("I attached you the exam photos to be able to analyze well");
            prompt.AppendLine("Please analyze these data and return the following according to students grades:");
            prompt.AppendLine("1- Hardest Question");
            prompt.AppendLine("2- Easiest Question");
            prompt.AppendLine("3- Hardest Subject");
            prompt.AppendLine("4- Easiest Subject");
            prompt.AppendLine("5- Avarage grade");
            prompt.AppendLine("6- Suggestions to enhance teachers skills in the subjects");
            prompt.AppendLine("7- Addtion to Suggestions, also suggest some activities that may enhance students understanding\n");

            prompt.AppendLine("You MUST return responses in the following JSON format EXACTLY::");
            prompt.AppendLine("    'Subjects':");
            prompt.AppendLine("    {");
            prompt.AppendLine("        'Hardest' : /* Map Students Data & Exam Data, and write the hardest subject title according the students answers*/,");
            prompt.AppendLine("        'Easiest' : /* Map Students Data & Exam Data, and write the easiest subject title according the students answers*/,");
            prompt.AppendLine("    },");
            prompt.AppendLine("    'Suggestions' : [ /* Write list of suggestions, each suggestion in 2 to 3 sentences write in deep to enhance the teachers skills and students socres & understanding to the subjects (if they where bad). Also give some advice about hardest subjects to students (At the end of the day the most improtant thing is enhancing students level)  */ ]");
            prompt.AppendLine("}");

            prompt.AppendLine("SUGGESTIONS INSTRUCRTIONS:");
            prompt.AppendLine("Each suggestion must be written in 2 to 3 sentences");
            prompt.AppendLine("Don't use markdown to format the result. Write it puer text");
            prompt.AppendLine("Suggestions must focus on enhance teachers skills so students can understand the subject more");
            return prompt.ToString();
        }
    }
}
