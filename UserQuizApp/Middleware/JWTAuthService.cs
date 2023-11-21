using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserQuizApp.Interfaces;

namespace UserQuizApp.Middleware
{
    public class JWTAuthService : IAuthService
    {
        //HTTP context accessor dependency injection
        private readonly IHttpContextAccessor _contextAccessor;

        public JWTAuthService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;            
        }

        /// <summary>
        /// Method that checks the current request headers to find the bearer token
        /// </summary>
        /// <returns>True, if there is a token and it's valid, false if otherwise</returns>
        public bool ValidateUser()
        {
            if (_contextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
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

        /// <summary>
        /// Method that returns the name of the authorized user
        /// </summary>
        /// <returns>String name, that represents the name of the authorized user</returns>
        public string ValidatedUserName()
        {
            if (_contextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
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
