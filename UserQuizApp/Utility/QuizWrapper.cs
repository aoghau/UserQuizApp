using UserQuizApp.Data;

namespace UserQuizApp.Utility
{
    //A couple of more advanced utility classes that wrap Quiz entity so that it can be turned into a JSON in its entirety
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
