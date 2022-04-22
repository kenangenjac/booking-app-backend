using System.Security.Claims;
using blandus_backend.Interfaces;
using blandus_backend.Models.Review;

namespace blandus_backend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ReviewService(DataContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<Review> AddReview(Guid accommodationId, InReviewDto request)
        {
            var username = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            var accommodation = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == accommodationId);

            //bool isUserFound = false;
            //for (int i = 0; i < accommodation.Reservations.Count; i++)
            //{
            //    if (accommodation.Reservations[i].UserId == userId)
            //    {
            //        isUserFound = true;
            //        break;
            //    }
            //}

            //if(!isUserFound) throw new Exception("You can't leave a review for an accommodation You didn't visit.");

            //decimal rating = new();
            //foreach (var r in accommodation?.Reviews)
            //{
            //    rating += r.Grade;
            //}

            if (user is null || accommodation is null)
            {
                throw new Exception("Not found");
            }

            var review = new Review()
            {
                Comment = request.Comment,
                Grade = request.Grade,
                User = user,
                Accommodation = accommodation,
            };

            _context.Reviews.Add(review);
            user.Reviews?.Add(review);

            var reviews = await _context.Reviews
                .Where(r => r.AccommodationId == accommodationId).ToListAsync();

            var rating = reviews.Sum(r => r.Grade);

            accommodation.Rating = (rating + review.Grade) / (reviews.Count + 1);
            accommodation.Reviews?.Add(review);

            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<List<Review>> GetReviews(Guid id)
        {
            var reviews = await _context.Reviews
                .Where(r => r.AccommodationId == id)
                .Include(u => u.User)
                .Include(a => a.Accommodation)
                .ToListAsync();

            return reviews ?? throw new Exception("No reviews found"); ;
        }

        public async Task<Review> GetReview(Guid id)
        {
            var review = await _context.Reviews
                .Include(u => u.User)
                .Include(a => a.Accommodation)
                .FirstOrDefaultAsync(r => r.Id == id);

            return review ?? throw new Exception("No reviews found"); ;
        }

        public async Task<Review> UpdateReview(Guid id, InReviewDto request)
        {
            var reviewToUpdate = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (reviewToUpdate == null) throw new Exception("No reviews found");

            var username = _contextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (reviewToUpdate.UserId != user?.Id) throw new Exception("You can only update your own review");

            reviewToUpdate.Comment = request.Comment;
            reviewToUpdate.Grade = request.Grade;

            await _context.SaveChangesAsync();

            return await GetReview(id);
        }

        public async Task<string> DeleteReview(Guid id)
        {
            var reviewToDelete = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (reviewToDelete == null) throw new Exception("No reviews found");

            var username = _contextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (reviewToDelete.UserId != user?.Id) throw new Exception("You can only delete your own review");

            _context.Reviews.Remove(reviewToDelete);
            await _context.SaveChangesAsync();

            return "Review deleted successfully";
        }
    }
}
