namespace HiveCardAPI.Models
{
    public class TransactionDetails
    {
        public int Id { get; set; }
        public int StatementId { get; set; }
        public string TransactionDate { get; set; }
        public string PostDate { get; set; }
        public string Description { get; set; }
        public string? Amount { get; set; }

        public Statement Statement { get; set; }
    }
}
