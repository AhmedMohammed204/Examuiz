# Examuiz

[![Examuiz Mobile App](https://img.shields.io/badge/Mobile%20App-Examuiz_Flutter-blue?style=flat&logo=flutter)](https://github.com/MohamedHamdySoftwareEngineer/Examuiz)

A smart service for automated exam generation and student answer analysis, designed to empower educators with AI-driven insights.

## 🚀 Features

### 📝 AI-Powered Exam Generation
- **Customizable Exams**: Generate exams from textbooks (e.g, Calculus book) with options for:
  - Number of questions
  - Difficulty level (Easy/Medium/Hard)
  - Question types (MCQ/Open-ended)
  - Page range filters
- **Teacher-Friendly PDF Export**: Exams include an answer key table for quick grading.

### 📊 Student Answer Analysis
- **Performance Analytics**:
  - Identify hardest/easiest subjects in the exam based on students answers
- **AI Teaching Recommendations**:
  - Personalized suggestions to improve teaching methods
  - Actionable insights to address knowledge gaps
  - Suggestions to enhance students understanding

## 🧩 Architecture (4-Layer Design)
| Layer          | Description                                                                 |
|----------------|-----------------------------------------------------------------------------|
| **AI Layer**   | Integrates Gemini AI for question generation and answer analysis            |
| **Core Layer** | Contains business logic for exam rules, analytics algorithms                |
| **API Layer**  | RESTful endpoints using ASP.NET                                            |
| **Presentation** | [Flutter Mobile App](https://github.com/MohamedHamdySoftwareEngineer/Examuiz) | 

## 🛠 Tech Stack
- **Backend**: C#, ASP.NET Core
- **AI Engine**: Google Gemini
- **Mobile App**: Flutter (Separate Repo)


## 📱 Mobile App Integration
The Flutter presentation layer is available in a separate repository:
[Examuiz Flutter Mobile App](https://github.com/MohamedHamdySoftwareEngineer/Examuiz)


