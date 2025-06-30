using System.Diagnostics;

namespace HiveCardAPI.Models
{ 
    public class Statement
    {
        public int Id { get; set; }
        public string CreditCardId { get; set; }
        public string StatementMonth { get; set; }
        public string PaymentDueDate { get; set; }
        public string TotalAmountDue { get; set; }
        public string MinimumAmountDue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public CreditCard CreditCard { get; set; }
        public List<TransactionDetails> TransactionDetails { get; set; } = new();
    }
}
