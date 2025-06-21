namespace HiveCardAPI.Dtos
{
    public class UserCreateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; } // plaintext for now (no auth layer yet)
    }
}
