using HiveCardAPI.Data;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditCardsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CreditCardsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CreditCards
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cards = await _context.CreditCards
                .AsNoTracking()
                .ToListAsync();

            return Ok(cards);
        }

        // GET: api/CreditCards/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var card = await _context.CreditCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (card == null)
                return NotFound();

            return Ok(card);
        }

        // POST: api/CreditCards
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreditCard card)
        {
            card.CreatedAt = DateTime.UtcNow;

            _context.CreditCards.Add(card);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = card.Id }, card);
        }

        // PUT: api/CreditCards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreditCard card)
        {
            if (id != card.Id)
                return BadRequest();

            var existing = await _context.CreditCards.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.UserId = card.UserId;
            existing.ProductId = card.ProductId;
            existing.CardNumber = card.CardNumber;
            existing.Nickname = card.Nickname;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/CreditCards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _context.CreditCards.FindAsync(id);
            if (card == null)
                return NotFound();

            _context.CreditCards.Remove(card);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
