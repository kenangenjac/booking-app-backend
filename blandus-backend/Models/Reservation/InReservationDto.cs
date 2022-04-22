using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace blandus_backend.Models.Reservation
{
    public class InReservationDto
    {
        public int NumberOfCompanions { get; set; }

        public bool IsCompanionAChild { get; set; } = false;

        public int NumberOfDaysStaying { get; set; }
        public DateTime ArrivalDateTime { get; set; }

        public DateTime DepartureDateTime { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Guest Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string GuestEmail { get; set; } = string.Empty;

        [JsonIgnore]
        public Guid AccommodationId { get; set; }
        
        public Guid? UserId { get; set; }
    }
}
