namespace HiveCardAPI.Models
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CreditCardProduct> CreditCardProducts { get; set; } = new();
    }
}
