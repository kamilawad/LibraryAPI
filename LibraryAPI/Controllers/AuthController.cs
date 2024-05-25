using LibraryAPI.Data;
using LibraryAPI.DTOs;
using LibraryAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(LibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Auth/register
        ///     {
        ///         "username": "Kamil Awad",
        ///         "password": "password123"
        ///     }
        /// </remarks>
        /// <param name="userDTO">The user registration details.</param>
        /// <returns>Status code 201 if the user is created, 400 if the username is already taken.</returns>
        /// <response code="201">User created.</response>
        /// <response code="400">Username is already taken.</response>
        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDTO.Username))
            {
                return BadRequest("Username is already taken.");
            }

            var user = new User
            {
                Username = userDTO.Username
            };
            user.HashPassword(userDTO.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, "User created.");
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Auth/login
        ///     {
        ///         "username": "Kamil Awad",
        ///         "password": "password123"
        ///     }
        /// </remarks>
        /// <param name="userDTO">The user login details.</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="401">Invalid username or password.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDTO.Username);
            if (dbUser == null || !dbUser.VerifyPassword(userDTO.Password))
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, dbUser.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}