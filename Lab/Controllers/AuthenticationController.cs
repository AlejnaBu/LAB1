using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Lab.Models;
using Lab.Data;
using Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AuthenticationController(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user);
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                    await _userManager.UpdateAsync(user);

                    return Ok(new { Token = token, RefreshToken = refreshToken, Role = user.UserRole });
                }
                return Unauthorized("Invalid login attempt.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Exception during login: {ex.Message}" });
            }
        }

        // ✅ Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                UserRole = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // ✅ Nëse është doktor, shtojmë në tabelën Doktoret
                if (model.Role.ToLower() == "doktor")
                {
                    var doktori = new Doktori
                    {
                        Id = Guid.NewGuid(), // Krijo ID të re për doktorin
                        UserId = user.Id,    // Lidhja me UserId
                        Name = model.Username,
                        Email = model.Email
                    };

                    _dbContext.Doktoret.Add(doktori);
                    await _dbContext.SaveChangesAsync();
                }

                // ✅ Nëse është pacient, shtojmë në tabelën Pacientet
                if (model.Role.ToLower() == "patient")
                {
                    var pacienti = new Pacienti
                    {
                        Id = Guid.NewGuid(), // Krijo ID të re për pacientin
                        UserId = user.Id,    // Lidhja me UserId
                        Name = model.Username,
                        Email = model.Email
                    };

                    _dbContext.Pacientet.Add(pacienti);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        // ✅ Refresh Token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequest model)
        {
            var user = await _userManager.FindByIdAsync(model.UserName);
            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token.");
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new { Token = newAccessToken, RefreshToken = newRefreshToken });
        }

        // ✅ Funksione ndihmëse për JWT
        //private string GenerateJwtToken(User user)
        //{
        //var claims = new[]
        //{
        // new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //   new Claim(ClaimTypes.Role, user.UserRole)
        // };

        //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"]));

        //var token = new JwtSecurityToken(
        // issuer: _configuration["Jwt:Issuer"],
        // audience: _configuration["Jwt:Audience"],
        // claims,
        // expires: expires,
        //   signingCredentials: creds
        // );

        // return new JwtSecurityTokenHandler().WriteToken(token);
        // }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // 🔹 Kjo ndihmon që API të marrë ID e përdoruesit
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.UserRole)  // 🔹 Siguro që UserRole po ruhet
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
