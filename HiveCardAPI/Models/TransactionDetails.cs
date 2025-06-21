namespace HiveCardAPI.Models
{
    public class TransactionDetails
    {
        public int Id { get; set; }
        public int StatementId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string RawAmount { get; set; }

        public Statement Statement { get; set; }
    }
}
