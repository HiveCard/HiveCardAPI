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

        // GET: api/CreditCards/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            var cards = await _context.CreditCards
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            if (cards == null || !cards.Any())
                return NotFound($"No credit cards found for UserId {userId}");

            return Ok(cards);
        }

        // PUT: api/CreditCards/user/{userId}/card/{cardId}/nickname
        [HttpPut("user/{userId:int}/card/{cardId:int}/nickname")]
        public async Task<IActionResult> UpdateNickname([FromRoute] int userId, [FromRoute] int cardId, [FromBody] string newNickname)
        {
            if (string.IsNullOrWhiteSpace(newNickname))
                return BadRequest("Nickname cannot be empty.");

            var card = await _context.CreditCards
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == cardId);

            if (card == null)
                return NotFound($"CreditCard with Id {cardId} for UserId {userId} not found.");

            card.Nickname = newNickname;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Nickname updated successfully.", card.Id, card.Nickname });
        }

        // DELETE: api/CreditCards/user/{userId}/card/{cardNumber}
        [HttpDelete("user/{userId:int}/card/{cardNumber}")]
        public async Task<IActionResult> DeleteByUserIdAndCardNumber([FromRoute] int userId, [FromRoute] string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return BadRequest("CardNumber cannot be empty.");

            var card = await _context.CreditCards
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CardNumber == cardNumber);

            if (card == null)
                return NotFound($"CreditCard with CardNumber '{cardNumber}' for UserId {userId} not found.");

            _context.CreditCards.Remove(card);
            await _context.SaveChangesAsync();

            return Ok(new { message = "CreditCard deleted successfully.", card.Id, card.CardNumber });
        }

        // DELETE: api/CreditCards/by-user/{userId}
        [HttpDelete("by-user/{userId}")]
        public async Task<IActionResult> DeleteByUserId(int userId)
        {
            // 1️⃣ Find all CreditCards for this User.ID == CreditCard.UserId
            var creditCards = await _context.CreditCards
                .Where(cc => cc.UserId == userId)
                .ToListAsync();

            if (!creditCards.Any())
            {
                return NotFound($"No CreditCards found for UserId = {userId}");
            }

            foreach (var creditCard in creditCards)
            {
                // 2️⃣ Find all Statements where Statement.CreditCardId == CreditCard.CardNumber
                var statements = await _context.Statements
                    .Where(s => s.CreditCardId == creditCard.CardNumber)
                    .ToListAsync();

                foreach (var statement in statements)
                {
                    // 3️⃣ Find all TransactionDetails where TransactionDetails.StatementId == Statement.Id
                    var transactionDetails = await _context.TransactionDetail
                        .Where(td => td.StatementId == statement.Id)
                        .ToListAsync();

                    // Delete TransactionDetails
                    _context.TransactionDetail.RemoveRange(transactionDetails);
                }

                // Delete Statements
                _context.Statements.RemoveRange(statements);
            }

            // Finally, delete CreditCards
            _context.CreditCards.RemoveRange(creditCards);

            await _context.SaveChangesAsync();

            return Ok($"Deleted CreditCards, related Statements, and TransactionDetails for UserId = {userId}");
        }

    }
}
