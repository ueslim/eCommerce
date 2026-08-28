using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Usuário e senha fixos exigidos pelo teste prático [3]
            if (request.Email == "wes@tech.com" && request.Password == "Senha@123")
            {
                var token = GenerateJwtToken(request.Email);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string email)
        {
            var secretKey = _config["JWT_KEY"];
            var issuer = _config["JWT_ISSUER"];
            var audience = _config["JWT_AUDIENCE"];

            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("A chave JWT_KEY precisa estar configurada no .env com pelo menos 32 caracteres.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

    public record LoginRequest(string Email, string Password);
}
