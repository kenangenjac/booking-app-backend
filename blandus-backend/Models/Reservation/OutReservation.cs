namespace blandus_backend.Models.Reservation
{
    public class OutReservation
    {
        public Guid Id { get; set; }

        public int NumberOfCompanions { get; set; }

        public bool IsCompanionAChild { get; set; } = false;

        public int NumberOfDaysStaying { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public decimal TotalPrice { get; set; }

        public string ArrivalDate => ArrivalDateTime.ToString("MM/dd/yyyy");

        public string ArrivalTime => ArrivalDateTime.ToString("hh:mm tt");

        public string DepartureDate => DepartureDateTime.ToString("MM/dd/yyyy");

        public string DepartureTime => DepartureDateTime.ToString("hh:mm tt");

        public Guid AccommodationId { get; set; }

        public Guid? UserId { get; set; }

        public string? GuestEmail { get; set; } = string.Empty;
    }
}
