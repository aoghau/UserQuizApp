using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserQuizApp.Data;
using UserQuizApp.Utility;
using UserQuizApp.Interfaces;
using UserQuizApp.Middleware;
using UserQuizApp.Data.Interfaces;

namespace UserQuizApp.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _config;
        private IQuizDataContext _context;
        private IAuthService _auth;

        public HomeController(IConfiguration config, IQuizDataContext context, IAuthService auth)
        {
            _config = config;
            _context = context;
            _auth = auth;
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            var wrapper = new ListWrapper<Quiz>() { List = _context.GetQuizzes().ToArray() };
            if (_auth.ValidateUser())
            {
                var user = _context.GetUsers().Where(x => x.Name.Equals(_auth.ValidatedUserName())).FirstOrDefault();
                user.Quizzes = _context.GetQuizzes().Where(x => x.UserId == user.Id).ToList();
                var combwrap = new CombinedWrapper<Quiz>() { List = user.Quizzes.ToArray(), WrapName = user.Name };
                return new JsonResult(combwrap);
            }
            return new JsonResult(wrapper);
        }

        [HttpPost("assignment")]
        public IActionResult AssignmentSelection()
        {
            if (_auth.ValidateUser())
            {
                var quizzes = _context.GetQuizzes().ToArray();
                var wrapper = new ListWrapper<Quiz>() { List = quizzes };
                return new JsonResult(wrapper);
            }
            return new UnauthorizedResult();
        }



        [HttpPost("assign")]
        public IActionResult AssignQuiz(int id)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.Id == id).FirstOrDefault();
                var user = _context.GetUsers().Where(x => x.Name.Equals(_auth.ValidatedUserName())).FirstOrDefault();
                user.Quizzes.Add(quiz);
            }
            return new OkResult();
        }

        [HttpPost("pass")]
        public IActionResult PassQuiz(int id)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.Id == id).FirstOrDefault();
                quiz.IsCompleted = true;
            }
            return new OkResult();
        }

        [HttpGet("id")]
        public IActionResult LoadQuiz(int id)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.Id == id).FirstOrDefault();
                List<Question> quizQuestions = _context.GetQuestions().Where(x => x.QuizId == quiz.Id).ToList();
                List<QuestionWrapper> questionWrappers = new List<QuestionWrapper>();
                for(int i = 0; i < quizQuestions.Count; i++) 
                {
                    List<Answer> answers = _context.GetAnswers().Where(x => x.QuestionId == quizQuestions[i].Id).ToList();
                    QuestionWrapper question = new QuestionWrapper(){ QuestionWrapText = quizQuestions[i].QuestionText, QuestionAnswers = answers  };
                    questionWrappers.Add(question);
                }
                QuizWrapper quizWrapper = new QuizWrapper() { QuizWrapName = quiz.QuizName, List = questionWrappers};
                return new JsonResult(quizWrapper);
            }
            return new UnauthorizedResult();
        }

        [HttpPost("sample")]
        public IActionResult AddSample()
        {
            List<Quiz> quizzes = new List<Quiz>();
            List<Question> questions = new List<Question>();
            List<Answer> answers = new List<Answer>();

            User sample = _context.Users.Where(x => x.Name.Equals("user")).FirstOrDefault();
            sample.Quizzes = quizzes;
            Quiz quiz = new Quiz() { Id = 0, QuizName = "Sample Quiz", Questions = questions, UserId = 0, IsCompleted = false };
            Question first = new Question() { Id = 0, QuestionText = "Why did the chicken cross the road?", QuizId = 0 };
            Answer joke = new Answer() { Id = 0, AnswerText = "To get to the other side", IsCorrect = true, QuestionId = 0 };

            quizzes.Add(quiz);
            questions.Add(first);
            answers.Add(joke);
            
            _context.Quizzes.Add(quiz);
            _context.Questions.Add(first);
            _context.Answers.Add(joke);
            _context.SaveChanges();


            return new JsonResult(quiz);
        }

        [HttpPost("sample/big")]
        public IActionResult AddComplexSample()
        {
            List<Quiz> quizzes = new List<Quiz>();
            List<Question> questions = new List<Question>();
            List<Answer> answers = new List<Answer>();

            User sample = _context.Users.Where(x => x.Name.Equals("user")).FirstOrDefault();
            sample.Quizzes = quizzes;
            Quiz quiz = new Quiz() { Id = 2, QuizName = "Big quiz", Questions = questions, UserId = 0, IsCompleted = false };
            Question first = new Question() { Id = 123, QuestionText = "What is the unit of temperature measurement that is accepted in EU?", QuizId = 2 };
            Answer kelvin = new Answer() { Id = 111, AnswerText = "Kelvin", IsCorrect = false, QuestionId = 123 };
            Answer newton = new Answer() { Id = 112, AnswerText = "Newton", IsCorrect = false, QuestionId = 123 };
            Answer celsius = new Answer() { Id = 113, AnswerText = "Celsius", IsCorrect = true, QuestionId = 123 };
            Answer faren = new Answer() { Id = 114, AnswerText = "Farenheit", IsCorrect = false, QuestionId = 123 };
            Question second = new Question() { Id = 124, QuestionText = "What does fortissimo mean in music?", QuizId = 2 };
            Answer sync = new Answer() { Id = 115, AnswerText = "Playing in sync", IsCorrect = false, QuestionId = 124 };
            Answer loud = new Answer() { Id = 116, AnswerText = "Playing really loud", IsCorrect = true, QuestionId = 124 };
            Answer key = new Answer() { Id = 117, AnswerText = "Key change", IsCorrect = false, QuestionId = 124 };
            Answer time = new Answer() { Id = 118, AnswerText = "Playing double time", IsCorrect = false, QuestionId = 124 };

            quizzes.Add(quiz);
            questions.Add(first);
            questions.Add(second);
            answers.Add(kelvin);
            answers.Add(newton);
            answers.Add(celsius);
            answers.Add(faren);
            answers.Add(sync);
            answers.Add(loud);
            answers.Add(key);
            answers.Add(time);

            _context.Quizzes.AddRange(quizzes);
            _context.Questions.AddRange(questions);
            _context.Answers.AddRange(answers);
            _context.SaveChanges();

            return new JsonResult(quiz);
        }
    }
}
