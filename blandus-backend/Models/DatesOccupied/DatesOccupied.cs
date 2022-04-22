using System.Text.Json.Serialization;

namespace blandus_backend.Models.DatesOccupied
{
    public class DatesOccupied
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime ArrivalDateTime { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public string ArrivalDate => ArrivalDateTime.ToString("MM/dd/yyyy");

        public string ArrivalTime => ArrivalDateTime.ToString("hh:mm tt");

        public string DepartureDate => DepartureDateTime.ToString("MM/dd/yyyy");

        public string DepartureTime => DepartureDateTime.ToString("hh:mm tt");

        public Guid AccommodationId { get; set; }

        [JsonIgnore]
        public Accommodation.Accommodation? Accommodation { get; set; }
    }
}
