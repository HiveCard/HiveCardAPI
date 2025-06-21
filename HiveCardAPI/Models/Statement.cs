using System.Diagnostics;

namespace HiveCardAPI.Models
{ 
    public class Statement
    {
        public int Id { get; set; }
        public int CreditCardId { get; set; }
        public DateTime StatementMonth { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal MinimumAmountDue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public CreditCard CreditCard { get; set; }
        public List<TransactionDetails> TransactionDetails { get; set; } = new();
    }
}
