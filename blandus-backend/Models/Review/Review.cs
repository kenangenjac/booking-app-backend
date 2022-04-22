using System.Text.Json.Serialization;

namespace blandus_backend.Models.Review
{
    public class Review : InReviewDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CommentDateTime { get; set; } = DateTime.Now;

        // props for relationship configuration
        [JsonIgnore]
        public Accommodation.Accommodation? Accommodation { get; set; }
        [JsonIgnore]
        public User.User? User { get; set; }
    }
}
