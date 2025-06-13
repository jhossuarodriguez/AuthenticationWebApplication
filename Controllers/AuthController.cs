using AuthenticationWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AuthenticationWebApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly GestorDeUsuariosDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(GestorDeUsuariosDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null || !PasswordHelper.VerifyPassword(login.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Credenciales inválidas");

            if (user.IsLockedOut == true)
                return Unauthorized("Cuenta bloqueada.");
            if (user.IsActive == false)
                return Unauthorized("Cuenta inactiva.");

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        [Route("Auth/Register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
            {
                ModelState.AddModelError("", "El usuario o email ya existen.");
                return View(model);
            }

            // Genera salt y hash para la contraseña
            var salt = Guid.NewGuid().ToString();
            using var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(salt));
            var hash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password)));

            var user = new User
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault(); 
            if (authHeader == null || !authHeader.StartsWith("Bearer ")) return BadRequest("No se proporcionó un token de autenticación.");

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti)) return BadRequest("Token inválido.");

            _context.RevokedTokens.Add(new RevokedToken
            {
                Jti = jti,
                RevokedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sesión cerrada correctamente." });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var firstName = User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = User.FindFirstValue(ClaimTypes.Surname);
            var username = User.FindFirstValue(ClaimTypes.Name);

            return Ok(new
            {
                Id = userId,
                Username = username,
                FirstName = firstName,
                LastName = lastName
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "User"),
                    new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
                    new Claim(ClaimTypes.Surname, user.LastName ?? "")

                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}