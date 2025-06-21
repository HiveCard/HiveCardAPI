namespace HiveCardAPI.Models
{
    public class CreditCardProduct
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CardType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Bank Bank { get; set; }
        public List<CreditCard> CreditCards { get; set; } = new();
    }
}
