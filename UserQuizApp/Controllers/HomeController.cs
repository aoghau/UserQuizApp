using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
