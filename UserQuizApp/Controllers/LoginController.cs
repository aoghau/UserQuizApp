using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserQuizApp.Data;

namespace UserQuizApp.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private QuizDataContext _context;

        public LoginController(IConfiguration config, QuizDataContext quizDataContext)
        {
            _config = config;
            _context = quizDataContext;
        }

        [HttpPost("Login")]
        public IActionResult Login(string name, string password)
        {
            var isUser = IsAuthorizedUser(name, password);

            if (isUser)
            {
                var token = Generate(name, password);
                return Ok(token);
            }
            return NotFound();

        }

        private string Generate(string name, string password)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, name)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
             _config["Jwt:Audience"],
             claims,
             expires: DateTime.Now.AddMinutes(15),
             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private bool IsAuthorizedUser(string username, string password)
        {            
            if (_context.Users.Where(x => x.Name.Equals(username) && x.Password.Equals(password)).Single() != null)
            {
                return true;
            }
            return false;            
        }
    }
}
