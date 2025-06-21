using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HiveCardAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatementsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StatementsController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] UploadStatementDto dto)
        {
            var user = await _db.Users.FindAsync(dto.UserId);
            var card = await _db.CreditCards
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == dto.CreditCardId && c.UserId == dto.UserId);

            if (user == null || card == null)
                return BadRequest("Invalid user or credit card.");

            var statement = new Statement
            {
                CreditCardId = card.Id,
                StatementMonth = dto.StatementMonth,
                PaymentDueDate = dto.PaymentDueDate,
                TotalAmountDue = dto.TotalAmountDue,
                MinimumAmountDue = dto.MinimumAmountDue,
                TransactionDetails = dto.Activities.Select(a => new TransactionDetails
                {
                    TransactionDate = a.TransactionDate,
                    PostDate = a.PostDate,
                    Description = a.Description,
                    Amount = a.Amount,
                    RawAmount = a.RawAmount
                }).ToList()
            };

            _db.Statements.Add(statement);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Statement uploaded", statementId = statement.Id });
        }
    }
}
