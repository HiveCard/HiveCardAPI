namespace HiveCardAPI.Dtos
{
    public class UploadStatementDto
    {
        public int UserId { get; set; }
        public int CreditCardId { get; set; }
        public DateTime StatementMonth { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal MinimumAmountDue { get; set; }
        public List<ActivityDto> Activities { get; set; } = new();
    }
}
