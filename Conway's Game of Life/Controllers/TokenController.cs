using Conway_s_Game_of_Life.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace Conway_s_Game_of_Life.Controllers
{
    [ProducesResponseType(500)]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    [ApiController]
    [Route("token")]
    public class TokenController : Controller
    {

        private readonly JwtConfiguration _jwtConfiguration;

        public TokenController(JwtConfiguration jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration;
        }

        [ProducesResponseType(typeof(Token), 200)]
        [ProducesResponseType(401)]
        [HttpPost()]
        public IActionResult Post([FromHeader] String authorization, [FromForm] string scope)
        {
            var credentials = TokenUtils.ExtractFromBasic(authorization);
            String clientId = credentials.User;
            String clientSecret = credentials.Password;
            var client = JwtConfiguration.AllowedClients.Where(c => c.User == clientId).FirstOrDefault();
            if (client != null && client.Password == clientSecret)
            {

                var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret ?? "");
                var scopeList = scope.Split(" ").ToList();
                var additionalClaims = new Dictionary<string, object>() {
                    { ClaimConstants.Scope, scopeList.Intersect(JwtConfiguration.AllowedScopes).ToArray()}
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _jwtConfiguration.Issuer,
                    Audience = _jwtConfiguration.Audience,
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,clientId),
                        new Claim(ClaimTypes.Name, clientId)

                    }),
                    Claims = additionalClaims,
                    Expires = DateTime.UtcNow.AddSeconds(_jwtConfiguration.ExpirationInSeconds),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenResponse = new Token("Bearer", tokenHandler.WriteToken(token), _jwtConfiguration.ExpirationInSeconds);
                return Ok(tokenResponse);

            }
            return Unauthorized();

        }
    }
}
