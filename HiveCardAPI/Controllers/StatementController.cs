using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;


namespace HiveCardAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatementsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StatementsController(AppDbContext db) => _db = db;

        [HttpGet("card/{creditCardId}/latest")]
        public async Task<IActionResult> GetLatestStatement([FromRoute] string creditCardId)
        {
            if (string.IsNullOrWhiteSpace(creditCardId))
                return BadRequest("CreditCardId cannot be empty.");

            var statements = await _db.Statements
                .Where(s => s.CreditCardId == creditCardId)
                .AsNoTracking()
                .ToListAsync();

            if (!statements.Any())
                return NotFound($"No statements found for CreditCardId '{creditCardId}'.");

            // Parse the StatementMonth strings to DateTime for comparison
            var latest = statements
                .Select(s => new
                {
                    Statement = s,
                    ParsedDate = DateTime.TryParseExact(
                        s.StatementMonth,
                        new[] { "dd-MMM-yy", "dd MMMM yyyy", "yyyy-MM-dd", "MMMM dd, yyyy", "MMM dd yyyy" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsed) ? parsed : DateTime.MinValue
                })
                .OrderByDescending(x => x.ParsedDate)
                .FirstOrDefault();

            if (latest == null || latest.ParsedDate == DateTime.MinValue)
                return NotFound($"No valid StatementMonth format for CreditCardId '{creditCardId}'.");

            return Ok(latest.Statement);
        }

    }
}
