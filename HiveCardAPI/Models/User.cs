﻿namespace HiveCardAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CpNumber { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CreditCard> CreditCards { get; set; } = new();
    }


}
