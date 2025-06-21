namespace HiveCardAPI.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string CardNumber { get; set; }
        public string Nickname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public CreditCardProduct Product { get; set; }
        public List<Statement> Statements { get; set; } = new();
    }
}
