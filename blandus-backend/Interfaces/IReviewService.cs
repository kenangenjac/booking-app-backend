using blandus_backend.Models.Review;

namespace blandus_backend.Interfaces
{
    public interface IReviewService
    {
        public Task<Review> AddReview(Guid id, InReviewDto request);
        public Task<List<Review>> GetReviews(Guid id);
        public Task<Review> GetReview(Guid id);
        public Task<Review> UpdateReview(Guid id, InReviewDto request);
        public Task<string> DeleteReview(Guid id);
    }
}
