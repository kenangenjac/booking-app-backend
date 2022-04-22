using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace blandus_backend.Models.Accommodation
{
    public class Accommodation : InAccommodationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "decimal(3,1)")]
        public decimal Rating { get; set; }

        // props for relationship configuration
        [JsonIgnore]
        public User.User? User { get; set; }

        public List<Service.Service>? Services { get; set; } = new();
        public List<Review.Review>? Reviews { get; set; } = new();
        public List<Image.Image>? Images { get; set; } = new();
        public List<Reservation.Reservation>? Reservations { get; set; } = new();
        public List<DatesOccupied.DatesOccupied>? DatesOccupied { get; set; } = new();
    }
}
