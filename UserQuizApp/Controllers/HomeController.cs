using Microsoft.AspNetCore.Mvc;
using UserQuizApp.Data;
using UserQuizApp.Utility;

namespace UserQuizApp.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Home()
        {
            using(var context = new QuizDataContext())
            {
               // var wrapper = new ListWrapper<Quiz>
            }
            return new JsonResult("");
        }

        [HttpPost("sample")]
        public IActionResult AddSample()
        {
            List<Quiz> quizzes = new List<Quiz>();
            List<Question> questions = new List<Question>();
            List<Answer> answers = new List<Answer>();

            User sample = new User() { Id = 0, Name = "John Doe", Password = "password", Quizzes = quizzes };
            Quiz quiz = new Quiz() { Id = 0, QuizName = "Sample Quiz", Questions = questions, User = sample, UserId = 0, IsCompleted = false};
            Question first = new Question() { Id = 0, QuestionText = "Why did the chicken cross the road?", Quiz = quiz, QuizId = 0 };
            Answer joke = new Answer() { Id = 0, AnswerText = "To get to the other side", IsCorrect = true, Question = first, QuestionId = 0 };

            quizzes.Add(quiz);
            questions.Add(first);
            answers.Add(joke);

            using (var context = new QuizDataContext())
            {
                context.Users.Add(sample);
                context.Quizzes.Add(quiz);
                context.Questions.Add(first);
                context.Answers.Add(joke);
                context.SaveChanges();
            }
            
            return new JsonResult(quiz);
        }
        
    }
}
