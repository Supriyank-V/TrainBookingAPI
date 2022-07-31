namespace TrainBookingAPI.Models
{
    public class BookingDetails
    {
        public int Id { get; set; }
        public int IdBooking { get; set; }
        public string? PassengerName { get; set; }
        public int Age { get; set; }
        public int PhoneNumber { get; set; }
        public string? EmailId { get; set; }
    }
}
