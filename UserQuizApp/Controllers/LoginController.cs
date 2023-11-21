using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserQuizApp.Data;
using UserQuizApp.Data.Interfaces;

namespace UserQuizApp.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private IQuizDataContext _context;

        public LoginController(IConfiguration config, IQuizDataContext quizDataContext)
        {
            _config = config;
            _context = quizDataContext;            
        }

        /// <summary>
        /// Creates a sample user to test the application with
        /// </summary>
        /// <returns>OkObject with the created user inside to check it just in case</returns>
        [HttpGet("sample")]
        public IActionResult CreateSampleUser() 
        {
            User sample = new User() { Id = 0, Name = "user", Password = "password" };
            _context.Users.Add(sample);
            _context.SaveChanges();
            return new OkObjectResult(sample);
        }

        /// <summary>
        /// Loging in, action then returns a JWT bearer token that frontend can work with
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns>Not found if there is no such user, Ok with token if there is</returns>
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

        /// <summary>
        /// Generates the token
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns>JWT token</returns>
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


        /// <summary>
        /// Checks if there is such a user in database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>True if user exists, false if not</returns>
        private bool IsAuthorizedUser(string username, string password)
        {            
            if (_context.Users.Where(x => x.Name.Equals(username) && x.Password.Equals(password))?.Single() != null)
            {
                return true;
            }
            return false;            
        }
    }
}
