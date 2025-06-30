using HiveCardAPI.Data;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-statement/{statementId}")]
        public async Task<ActionResult<IEnumerable<TransactionDetails>>> GetByStatementId(int statementId)
        {
            var results = await _context.TransactionDetail
                .Where(td => td.StatementId == statementId)
                .ToListAsync();

            if (results == null || results.Count == 0)
            {
                return NotFound($"No TransactionDetails found for StatementId = {statementId}");
            }

            return Ok(results);
        }
    }
}
