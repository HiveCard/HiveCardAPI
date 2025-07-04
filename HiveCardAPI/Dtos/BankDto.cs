namespace HiveCardAPI.Dtos
{
    public class BankCreateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class CreditCardProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CardType { get; set; }
    }

    public class BankDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<CreditCardProductDto> CreditCardProducts { get; set; } = new();
    }
}
