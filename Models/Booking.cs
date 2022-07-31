namespace TrainBookingAPI.Models
{
    public class Booking
    {
        public int IdTrain { get; set; }
        public string TrainName { get; set; }  = string.Empty;
        public int IdSubTrain { get; set; }
        public int IdUser { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? BookingFor { get; set; }
        public string? Initial { get; set; }
        public string? Destination { get; set; }
        public int NoOfSeats { get; set; }
        public int IdSubTrainCoach { get; set; }
        public int Fare { get; set; }
        public string? PaymentMethod { get; set; }
        public string? MealPreference { get; set; }
        public List<Passengers>? Passengers { get; set; }
    }
}
