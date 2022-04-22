using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace blandus_backend.Models.Reservation
{
    public class Reservation : InReservationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [NotMapped]
        public string ArrivalDate => ArrivalDateTime.ToString("MM/dd/yyyy");

        [NotMapped]
        public string ArrivalTime => ArrivalDateTime.ToString("hh:mm tt");

        [NotMapped]
        public string DepartureDate => DepartureDateTime.ToString("MM/dd/yyyy");

        [NotMapped]
        public string DepartureTime => DepartureDateTime.ToString("hh:mm tt");

        // props for relationship configuration
        [JsonIgnore]
        public Accommodation.Accommodation? Accommodation { get; set; }
        [JsonIgnore]
        public User.User? User { get; set; }
    }
}
