namespace HiveCardAPI.Dtos
{
    public class ActivityDto
    {
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string RawAmount { get; set; }
    }
}
