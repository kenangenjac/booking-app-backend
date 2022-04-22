using System.ComponentModel.DataAnnotations;

namespace blandus_backend.Models.User
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;


        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required.")]
        public byte[]? PasswordHash { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public byte[]? PasswordSalt { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Role { get; set; } = "User";

        // props for relationship configuration
        public List<Accommodation.Accommodation>? Accommodations { get; set; } = new();
        public List<Review.Review>? Reviews { get; set; } = new();
        public List<Reservation.Reservation>? Reservations { get; set; } = new();
    }
}
