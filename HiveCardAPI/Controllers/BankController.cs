using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BanksController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> CreateBank([FromBody] BankCreateDto dto)
        {
            if (await _db.Banks.AnyAsync(b => b.Code == dto.Code))
                return BadRequest(new { message = "Bank code already exists." });

            var bank = new Bank
            {
                Name = dto.Name,
                Code = dto.Code
                // CreatedAt is automatically set by the model
            };

            _db.Banks.Add(bank);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBankById),
                new { id = bank.Id },
                new { message = "Bank created successfully", bank });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBanks()
        {
            var banks = await _db.Banks.ToListAsync();
            return Ok(banks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            var bank = await _db.Banks.FindAsync(id);

            if (bank == null)
                return NotFound(new { message = $"Bank with ID {id} not found." });

            return Ok(bank);
        }
    }
}
