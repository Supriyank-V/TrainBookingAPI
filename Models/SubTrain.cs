namespace TrainBookingAPI.Models
{
    public class SubTrain
    {
        public int Id { get; set; }
        public int IdTrain { get; set; }
        public DateOnly DateOfDeparture { get; set; }
        public DateOnly DateOfArrival { get; set; }
        public int FirstClass { get; set; }
        public int SecondClass { get; set; }
        public int ThridAC { get; set; }
        public int Sleeper { get; set; }
        public int CC { get; set; }
    }
}
