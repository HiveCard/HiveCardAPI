using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db) => _db = db;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { error = "Email and password are required." });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { error = "User not found." });

            if (user.Password != request.Password)
                return Unauthorized(new { error = "Invalid password." });

            return Ok(new
            {
                userId = user.Id,
                email = user.Email,
                fullName = user.Name
            });
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already in use.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                CpNumber = dto.PhoneNumber,
                Password = dto.Password // NOTE: plain for now, hash later
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return Ok($"User with ID {id} has been deleted.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(user);
        }

        [HttpGet("exists/by-email")]
        public async Task<IActionResult> CheckUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email is required." });

            var exists = await _db.Users.AnyAsync(u => u.Email == email);
            return Ok(new { exists });
        }
    }
}