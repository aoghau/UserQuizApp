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

        [HttpPost("assign")]
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



        [HttpPost("assign/{quiz}")]
        public IActionResult AssignQuiz(string quizname)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.QuizName.Equals(quizname)).FirstOrDefault();
                var user = _context.GetUsers().Where(x => x.Name.Equals(_auth.ValidatedUserName())).FirstOrDefault();
                user.Quizzes.Add(quiz);
            }
            return new OkResult();
        }

        [HttpPost("pass/{quiz}")]
        public IActionResult PassQuiz(string quizname)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.QuizName.Equals(quizname)).FirstOrDefault();
                quiz.IsCompleted = true;
            }
            return new OkResult();
        }

        [HttpGet("{quiz}")]
        public IActionResult LoadQuiz(string quizname)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.QuizName.Equals(quizname)).FirstOrDefault();
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

            User sample = new User() { Id = 0, Name = "user", Password = "password", Quizzes = quizzes };
            Quiz quiz = new Quiz() { Id = 0, QuizName = "Sample Quiz", Questions = questions, user = sample, UserId = 0, IsCompleted = false };
            Question first = new Question() { Id = 0, QuestionText = "Why did the chicken cross the road?", Quiz = quiz, QuizId = 0 };
            Answer joke = new Answer() { Id = 0, AnswerText = "To get to the other side", IsCorrect = true, Question = first, QuestionId = 0 };

            quizzes.Add(quiz);
            questions.Add(first);
            answers.Add(joke);


            _context.Users.Add(sample);
            _context.Quizzes.Add(quiz);
            _context.Questions.Add(first);
            _context.Answers.Add(joke);
            _context.SaveChanges();


            return new JsonResult(quiz);
        }
    }
}
