using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using milestone2.DTOs;
using milestone2.Data;
using milestone2.Models;

namespace milestone2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Username);

            if (user == null || dto.Password != "password") 
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user.Email, user.Role);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
