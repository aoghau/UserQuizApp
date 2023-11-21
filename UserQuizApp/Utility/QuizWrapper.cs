using UserQuizApp.Data;

namespace UserQuizApp.Utility
{
    public class QuizWrapper
    {
        public string QuizWrapName { get; set; }
        public List<QuestionWrapper> List { get; set; }
        public QuizWrapper() { }
    }

    public class QuestionWrapper
    {
        public string QuestionWrapText { get; set; }
        public List<Answer> QuestionAnswers { get; set; }
        public QuestionWrapper() { }
    }
}
