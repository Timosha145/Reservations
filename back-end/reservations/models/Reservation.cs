namespace reservations.models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public string? Salon { get; set; }
        public string? CarNumber { get; set; }

        private Reservation() { }

        public Reservation (int clientId, int serviceId, DateTime date, string? salon, string? carNumber)
        {
            ClientId = clientId;
            ServiceId = serviceId;
            Date = date;
            Salon = salon;
            CarNumber = carNumber;
        }
    }
}
