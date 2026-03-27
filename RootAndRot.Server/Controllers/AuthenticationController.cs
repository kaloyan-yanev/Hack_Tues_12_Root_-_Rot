using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RootAndRot.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IAuthenticationService = RootAndRot.Server.Services.IAuthenticationService;   

namespace RootAndRot.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _config;

        public AuthenticationController(
            IAuthenticationService authenticationService, IConfiguration config)
        {
            _authenticationService = authenticationService;
            _config = config;
        }

        // ── POST /Authentication/Register ─────────────────────────
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO dto)
        {
            var success = await _authenticationService.Register(dto.Name, dto.Password);
            if (!success)
                return Conflict("A user with that name already exists.");

            return Ok();
        }

        // ── POST /Authentication/Login ───────────────────────────
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] RegistrationDTO dto)
        {
            var user = await _authenticationService.LogIn(dto.Name, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            // Issue token pair and save refresh token in DB
            (string accessToken, string refreshToken) =
                await _authenticationService.IssueTokenPair(user, _config["JwtKey"], "RootAndRot.Server", "RootAndRot.Server");

            return Ok(new { accessToken, refreshToken });
        }

        // ── POST /Authentication/Refresh ─────────────────────────
        [HttpPost("Refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDTO req)
        {
            var refreshTokenEntry = await _authenticationService.GetRefreshToken(req.RefreshToken);
            if (refreshTokenEntry == null)
                return Unauthorized("Unknown refresh token.");

            if (refreshTokenEntry.Consumed)
                return Unauthorized("Refresh token already used.");

            if (refreshTokenEntry.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token expired.");

            // Rotate: mark old token consumed and issue new pair
            refreshTokenEntry.Consumed = true;
            await _authenticationService.SaveRefreshToken(refreshTokenEntry);
            
            var user = refreshTokenEntry.User;
            var (accessToken, refreshToken) =
                await _authenticationService.IssueTokenPair(user, _config["JwtKey"], "RootAndRot.Server", "RootAndRot.Server");

            return Ok(new { accessToken, refreshToken });
        }

        // ── POST /Authentication/Logout ──────────────────────────
        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var id))
                return BadRequest();

            var success = await _authenticationService.LogOut(id);
            if (!success)
                return NotFound("User not found.");

            return Ok();
        }
        public static string CreateAccessToken(
            string name, string key, string issuer, string audience)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(ClaimTypes.Name, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string CreateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    

    }
}