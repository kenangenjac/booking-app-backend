using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blandus_backend.Models.Accommodation
{
    public class InAccommodationDto
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(350)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string RoomType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Contact { get; set; } = string.Empty;

        public double Price { get; set; }

        [Range(1, 6, ErrorMessage = "This field has a range of 1-6")]
        public int StarRating { get; set; }

        [NotMapped]
        public string[] ServicesArray { get; set; } = Array.Empty<string>();

        [NotMapped]
        public string[] ImagesArray { get; set; } = Array.Empty<string>();

        public Guid UserId { get; set; }
    }
}
