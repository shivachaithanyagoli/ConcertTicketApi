using ConcertTicketApi.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConcertTicketApi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // TODO: replace with real user store
            if (dto.Username == "admin" && dto.Password == "p@ssw0rd")
            {
                var auth = _config.GetSection("Auth");
                var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(auth["Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: auth["Issuer"],
                    audience: auth["Audience"],
                    claims: new[] { new Claim(ClaimTypes.Name, dto.Username) },
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
        }
    }
}
