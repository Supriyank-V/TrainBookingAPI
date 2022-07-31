namespace TrainBookingAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AccessType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Login { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set;} = string.Empty;
        public bool IsNortification { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }
    }
}
