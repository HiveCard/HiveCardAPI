using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db) => _db = db;

        [HttpPost]
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

        [HttpGet]
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