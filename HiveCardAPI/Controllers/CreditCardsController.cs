using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
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



        // POST: api/CreditCards
        //[HttpPost]
        //public async Task<IActionResult> CreateCreditCard([FromBody] CreditCardDto creditCardDto)
        //{
        //    if (creditCardDto == null)
        //        return BadRequest("Invalid payload.");

        //    if (string.IsNullOrWhiteSpace(creditCardDto.CardNumber))
        //        return BadRequest("CardNumber cannot be empty.");

        //    var creditCardProductId = creditCardDto.CreditCardProductId;

        //    if (creditCardProductId == null || creditCardProductId <= 0)
        //    {
        //        creditCardProductId = 1; // 👈 use a safe fallback valid ID that you know exists
        //    }


        //    //// ✅ Validate the linked CreditCardProduct exists
        //    //var productExists = await _context.CreditCardProducts
        //    //    .AnyAsync(p => p.Id == creditCardDto.CreditCardProductId);

        //    //if (!productExists)
        //    //    return BadRequest($"Invalid CreditCardProductId: {creditCardDto.CreditCardProductId}");

        //    // ✅ Check if this UserId + CardNumber pair already exists
        //    var exists = await _context.CreditCards
        //        .AnyAsync(c => c.UserId == creditCardDto.UserId && c.CardNumber == creditCardDto.CardNumber);

        //    if (exists)
        //        return Conflict($"A credit card with this type already exists for UserId {creditCardDto.UserId}.");

        //    var newCard = new CreditCard
        //    {
        //        UserId = creditCardDto.UserId,
        //        CreditCardProductId = creditCardProductId,
        //        CardNumber = creditCardDto.CardNumber.Trim(),
        //        Nickname = "extracting text (10+ mins)",
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.CreditCards.Add(newCard);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetByUserId), new { userId = newCard.UserId }, newCard);
        //}



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


            // ✅ Load all Statements for this card
            var statements = await _context.Statements
                .Where(s => s.CreditCardId == card.CardNumber)
                .ToListAsync();

            foreach (var statement in statements)
            {
                // ✅ Load TransactionDetails for this Statement
                var transactions = await _context.TransactionDetail
                    .Where(td => td.StatementId == statement.Id)
                    .ToListAsync();

                // ✅ Remove TransactionDetails
                _context.TransactionDetail.RemoveRange(transactions);
            }

            // ✅ Remove Statements
            _context.Statements.RemoveRange(statements);

            // ✅ Remove the CreditCard
            _context.CreditCards.Remove(card);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = "CreditCard deleted successfully.", 
                card.Id, 
                card.CardNumber,
                deletedStatements = statements.Count
            });
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
