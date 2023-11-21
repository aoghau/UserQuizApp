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
        //data context dependency injection
        private IQuizDataContext _context;
        //authorization service dependency injection
        private IAuthService _auth;

        public HomeController(IQuizDataContext context, IAuthService auth)
        {            
            _context = context;
            _auth = auth;
        }

        /// <summary>
        /// Action that returns JSON with all the quizzes that user has. If user is not logged in, returns all quizzes - but further interaction requires authorization
        /// </summary>
        /// <returns>JSON result with an array of quizzes inside</returns>
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

        /// <summary>
        /// Action returns all quizzes to assign to user
        /// </summary>
        /// <returns>JSON with quizzes, or Unauthorized if not logged in</returns>
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


        /// <summary>
        /// Action that assigns quiz to a user, so that it appears when Home() action is called
        /// </summary>
        /// <param name="id"></param>
        /// <returns>OkResult in all cases, this action is not accessible to unauthorized users</returns>
        [HttpPost("assign")]
        public IActionResult AssignQuiz(int id)
        {
            if (_auth.ValidateUser())
            {
                Quiz quiz = _context.GetQuizzes().Where(x => x.Id == id).FirstOrDefault();
                var user = _context.GetUsers().Where(x => x.Name.Equals(_auth.ValidatedUserName())).FirstOrDefault();
                user.Quizzes.Add(quiz);
                _context.SaveChanges();
            }
            return new OkResult();
        }

        /// <summary>
        /// Action that sets a quiz to completed
        /// </summary>
        /// <param name="id"></param>
        /// <returns>OkResult in all cases, this action is not accessible to unauthorized users</returns>
        [HttpPost("pass")]
        public IActionResult PassQuiz(int id)
        {
            if (_auth.ValidateUser())
            {
                _context.GetQuizzes().Where(x => x.Id == id).FirstOrDefault().IsCompleted = true;
                
                _context.SaveChanges();
            }
            return new OkResult();
        }

        /// <summary>
        /// Returns all the info about the quiz, including all questions and all answers
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JSON with an array of questions that contains an array of answers</returns>
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

        /// <summary>
        /// Action that adds a sample quiz with a question to the database for testing purposes
        /// </summary>
        /// <returns>JSON with the created quiz</returns>
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

        /// <summary>
        /// Action that creates a more complex quiz with a couple of questions and multiple answers and adds it to the database for testing purposes
        /// </summary>
        /// <returns>JSON with the quiz</returns>
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
