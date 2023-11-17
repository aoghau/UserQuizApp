using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserQuizApp.Data;
using UserQuizApp.Utility;
using UserQuizApp.Interfaces;

namespace UserQuizApp.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _config;
        private QuizDataContext _context;
        private IAuthService _auth;

        public HomeController(IConfiguration config, QuizDataContext context, IAuthService auth)
        {
            _config = config;
            _context = context;
            _auth = auth;
        }

        [HttpGet("home")]
        public IActionResult Home()
        {            
            var wrapper = new ListWrapper<Quiz>() { List = _context.Quizzes.ToArray() };
            if(ValidateUser())
            {
                var user = _context.Users.Where(x => x.Name.Equals(ValidatedUserName())).FirstOrDefault();
                user.Quizzes = _context.Quizzes.Where(x => x.UserId == user.Id).ToList();
                var combwrap = new CombinedWrapper<Quiz>() { List = user.Quizzes.ToArray(), WrapName = user.Name };
                return new JsonResult(combwrap);
            }
            return new JsonResult(wrapper);                       
        }

        [HttpPost("assign")]
        public IActionResult AssignmentSelection()
        {
            if(ValidateUser())
            {                
                var quizzes = _context.Quizzes.ToArray();
                var wrapper = new ListWrapper<Quiz>(){ List = quizzes};
                return new JsonResult(wrapper);                
            }
            return new JsonResult(null);
        }
        
        [HttpPost("assign/{quiz}")]
        public IActionResult AssignQuiz(string quizname)
        {            
            if(_auth.ValidateUser()) 
            {                
                Quiz quiz = _context.Quizzes.Where(x => x.QuizName.Equals(quizname)).FirstOrDefault();
                var user = _context.Users.Where(x => x.Name.Equals(ValidatedUserName())).FirstOrDefault();
                user.Quizzes.Add(quiz);                
            }
            return new OkResult();
        }

        [HttpPost("sample")]
        public IActionResult AddSample()
        {
            List<Quiz> quizzes = new List<Quiz>();
            List<Question> questions = new List<Question>();
            List<Answer> answers = new List<Answer>();

            User sample = new User() { Id = 0, Name = "user", Password = "password", Quizzes = quizzes };
            Quiz quiz = new Quiz() { Id = 0, QuizName = "Sample Quiz", Questions = questions, User = sample, UserId = 0, IsCompleted = false};
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


        private bool ValidateUser()
        {
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.FirstOrDefault()?.Replace("bearer ", ""); // Remove "Bearer " prefix

                if (!string.IsNullOrEmpty(token))
                {
                    var jwtHandler = new JwtSecurityTokenHandler();

                    var parsedToken = jwtHandler.ReadJwtToken(token);
                    var claims = parsedToken.Claims;

                    // Check if the token contains specific claim to identify an authenticated user
                    var isAuthenticated = claims.Any(c => c.Type == ClaimTypes.NameIdentifier);

                    // Create an info object
                    var info = new { IsAuthenticated = isAuthenticated };

                    return isAuthenticated; // Return the info object in JSON format
                }
            }
            return false;
        }

        private string ValidatedUserName()
        {
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.FirstOrDefault()?.Replace("bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var parsedToken = jwtHandler.ReadJwtToken(token);
                    Claim usernameClaim = parsedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                    if (usernameClaim != null)
                    {
                        string validatedname = usernameClaim.Value;
                        return validatedname;
                    }
                }
            }
            return null;
        }
    }
}
