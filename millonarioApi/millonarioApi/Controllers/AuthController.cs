using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para .AnyAsync y .SingleOrDefaultAsync
using Millonario.Infrastructure; // Para usar ApplicationDbContext
using Millonario.Domain; // Para usar tu clase Usuario
using millonarioApi.Models; // Para tus DTOs (RegisterRequest, LoginRequest, AuthResponse)

// *** Usings para JWT ***
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt; // Para JWT (JwtSecurityTokenHandler)
using System.Security.Claims; // Para Claims (Claim, ClaimsIdentity, ClaimTypes)
using Microsoft.IdentityModel.Tokens; // Para SymmetricSecurityKey, SecurityTokenDescriptor
using System.Text; // Para Encoding.UTF8
using Microsoft.Extensions.Configuration; // Para IConfiguration (para leer appsettings.json)

namespace millonarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // Para acceder a appsettings.json

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna errores de validación del DTO
            }

            // Verificar si el nombre de usuario ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new AuthResponse { Success = false, Message = "El nombre de usuario ya existe." });
            }

            // --- ¡ADVERTENCIA! ESTA LÍNEA ALMACENA LA CONTRASEÑA EN TEXTO PLANO ---
            // Para un trabajo académico muy simple y consciente del riesgo, se puede aceptar.
            // En un entorno real, esto sería una vulnerabilidad de seguridad crítica.
            var newUser = new Usuario
            {
                Username = request.Username,
                PasswordHash = request.Password, // Almacena la contraseña directamente (NO RECOMENDADO EN PRODUCCIÓN)
                Email = request.Email,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync(); // Guarda el nuevo usuario en la base de datos

            return Ok(new AuthResponse { Success = true, Message = "Registro exitoso." });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized(new AuthResponse { Success = false, Message = "Credenciales inválidas." });
            }

            // --- ¡ADVERTENCIA! ESTA LÍNEA COMPARA CONTRASEÑAS EN TEXTO PLANO ---
            // En un entorno real, esto sería una vulnerabilidad de seguridad crítica.
            if (user.PasswordHash != request.Password) // Compara directamente
            {
                return Unauthorized(new AuthResponse { Success = false, Message = "Credenciales inválidas." });
            }

            // Si las credenciales son válidas, generar un JWT
            var token = GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Inicio de sesión exitoso.",
                Username = user.Username,
                Email = user.Email,
                Token = token // El token JWT
            });
        }

        // Método privado para generar el JWT
        private string GenerateJwtToken(Usuario user)
        {
            // Obtener la configuración JWT de appsettings.json
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // Crear los Claims (afirmaciones) que contendrá el token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Describir cómo se creará el token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // El token expira en 1 hora
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Crear el token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}