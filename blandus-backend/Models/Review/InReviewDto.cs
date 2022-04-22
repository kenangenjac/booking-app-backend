using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blandus_backend.Models.Review
{
    public class InReviewDto
    {
        [MaxLength(350, ErrorMessage = "You have exceeded the maximum length of this field")]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "This field has a range of 1-5")]
        [Column(TypeName = "decimal(3,1)")]
        public decimal Grade { get; set; } 

        public Guid AccommodationId { get; set; }
        public Guid UserId { get; set; }
    }
}
