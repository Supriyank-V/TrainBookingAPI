namespace TrainBookingAPI.Models
{
    public class Train
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TrainCode { get; set; }
        public string? Initial { get; set; }
        public string? Destination { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }

    }
}
