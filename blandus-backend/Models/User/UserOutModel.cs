using System.ComponentModel.DataAnnotations;

namespace blandus_backend.Models
{
    public class UserOutModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public List<Accommodation.Accommodation>? Accommodations { get; set; } = new();

        public List<Review.Review>? Reviews { get; set; } = new();

        public List<Reservation.Reservation>? Reservations { get; set; } = new();
    }
}
