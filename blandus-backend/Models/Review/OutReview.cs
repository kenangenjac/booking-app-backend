namespace blandus_backend.Models.Review
{
    public class OutReview
    {
        public Guid Id { get; set; }

        public string Comment { get; set; } = string.Empty;
        public string CommentDate { get; set; } = string.Empty;

        public decimal Grade { get; set; }

        public Guid AccommodationId { get; set; }

        public Guid UserId { get; set; }

        public string Reviewer { get; set; } = string.Empty;
    }
}
