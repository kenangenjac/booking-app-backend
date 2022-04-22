using blandus_backend.Interfaces;
using blandus_backend.Models.Accommodation;
using blandus_backend.Models.Reservation;
using blandus_backend.Models.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace blandus_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationController : ControllerBase
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IReservationService _reservationService;
        private readonly IReviewService _reviewService;

        public AccommodationController(IAccommodationService accommodationService, IReservationService reservationService, IReviewService reviewService)
        {
            _accommodationService = accommodationService;
            _reservationService = reservationService;
            _reviewService = reviewService;
        }

        // POST - Add new accommodation
        // id here resembles id of a user - a logged user can create an accommodation
        [HttpPost("/accommodations"), Authorize]
        public async Task<ActionResult<OutAccommodation>> AddAccommodation(InAccommodationDto request)
        {
            var accommodation = await _accommodationService.AddAccommodation(request);

            var outAccommodation = new OutAccommodation()
            {
                Id = accommodation.Id,
                Contact = accommodation.Contact,
                Price = accommodation.Price,
                Description = accommodation.Description,
                City = accommodation.City,
                Address = accommodation.Address,
                RoomType = accommodation.RoomType,
                Name = accommodation.Name,
                Type = accommodation.Type,
                Owner = accommodation.User?.FirstName + " " + accommodation.User?.LastName,
                Services = accommodation.Services,
                Reviews = accommodation.Reviews,
                Images = accommodation.Images
            };

            return Ok(outAccommodation);
        }

        // GET - Get all accommodations
        [HttpGet("/accommodations")]
        public async Task<ActionResult<List<PaginationAccommodationResponse>>> GetAccommodations([FromQuery] string? search, [FromQuery] AccommodationFilter? filter, [FromQuery] int page, [FromQuery] int pageResults, [FromQuery] string? sort)
        {
            try
            {
                var accommodations = await _accommodationService.GetAccommodations(search, filter, page, pageResults, sort);

                return Ok(accommodations);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message});
            }
        }

        // GET - Get a specific accommodation
        [HttpGet("/accommodations/{id}")]
        public async Task<ActionResult<OutAccommodation>> GetAccommodation([FromRoute] Guid id)
        {
            try
            {
                var accommodation = await _accommodationService.GetAccommodation(id);

                var outAccommodation = new OutAccommodation()
                {
                    Id = accommodation.Id,
                    Contact = accommodation.Contact,
                    Price = accommodation.Price,
                    Description = accommodation.Description,
                    City = accommodation.City,
                    Address = accommodation.Address,
                    RoomType = accommodation.RoomType,
                    Name = accommodation.Name,
                    Type = accommodation.Type,
                    StarRating = accommodation.StarRating,
                    Rating = accommodation.Rating,
                    Owner = accommodation.User?.FirstName + " " + accommodation.User?.LastName,
                    Services = accommodation.Services,
                    Reviews = accommodation.Reviews,
                    Images = accommodation.Images,
                    Reservations = accommodation.Reservations,
                    DatesOccupied = accommodation.DatesOccupied,
                };

                return Ok(outAccommodation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // PUT - Update a specific accommodation
        [HttpPut("/accommodations/{id}"), Authorize]
        public async Task<ActionResult<OutAccommodation>>? UpdateAccommodation([FromRoute] Guid id, InAccommodationDto request)
        {
            try
            {
                var accommodation = await _accommodationService.UpdateAccommodation(id, request);

                var outAccommodation = new OutAccommodation()
                {
                    Id = accommodation.Id,
                    Contact = accommodation.Contact,
                    Price = accommodation.Price,
                    Description = accommodation.Description,
                    City = accommodation.City,
                    Address = accommodation.Address,
                    RoomType = accommodation.RoomType,
                    Name = accommodation.Name,
                    Type = accommodation.Type,
                    Owner = accommodation.User?.FirstName + " " + accommodation.User?.LastName,
                    Services = accommodation.Services,
                    Reviews = accommodation.Reviews,
                    Images = accommodation.Images,
                    Reservations = accommodation.Reservations,
                    DatesOccupied = accommodation.DatesOccupied,
                };

                return Ok(outAccommodation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // DELETE - Delete a specific accommodation
        [HttpDelete("/accommodations/{id}"), Authorize]
        public async Task<ActionResult>? DeleteAccommodation([FromRoute] Guid id)
        {
            try
            {
                var result = await _accommodationService.DeleteAccommodation(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(204, new { Message = e.Message });
            }
        }

        //RESERVATIONS

        // POST - Add new reservation
        [HttpPost("/accommodations/{id}/reservations/"), Authorize]
        public async Task<ActionResult<OutReservation>> AddReservation([FromRoute(Name = "id")] Guid accommodationId, InReservationDto request)
        {
            try
            {
                var reservation = await _reservationService.AddReservation(accommodationId, request);

                var outReservation = new OutReservation
                {
                    Id = reservation.Id,
                    NumberOfCompanions = reservation.NumberOfCompanions,
                    IsCompanionAChild = reservation.IsCompanionAChild,
                    NumberOfDaysStaying = reservation.NumberOfDaysStaying,
                    ArrivalDateTime = reservation.ArrivalDateTime,
                    DepartureDateTime = reservation.DepartureDateTime,
                    TotalPrice = reservation.TotalPrice,
                    GuestEmail = reservation.GuestEmail,
                    AccommodationId = reservation.AccommodationId,
                };

                if (reservation.UserId != null)
                {
                    outReservation.UserId = reservation.UserId;
                }

                return Ok(outReservation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // GET - Get all reservations
        [HttpGet("/accommodations/{id}/reservations/")]
        public async Task<ActionResult<List<OutReservation>>> GetReservations([FromRoute] Guid id)
        {
            try
            {
                var accommodation = await _accommodationService.GetAccommodation(id);
                var reservations = await _reservationService.GetReservations(id);

                var outReservations = new List<OutReservation>();

                foreach (var reservation in reservations)
                {
                    var outReservation = new OutReservation
                    {
                        Id = reservation.Id,
                        NumberOfCompanions = reservation.NumberOfCompanions,
                        IsCompanionAChild = reservation.IsCompanionAChild,
                        NumberOfDaysStaying = reservation.NumberOfDaysStaying,
                        ArrivalDateTime = reservation.ArrivalDateTime,
                        DepartureDateTime = reservation.DepartureDateTime,
                        TotalPrice = reservation.TotalPrice,
                        AccommodationId = reservation.AccommodationId,
                        GuestEmail = reservation.GuestEmail,
                    };

                    if (reservation.UserId != null)
                    {
                        outReservation.UserId = reservation.UserId;
                    }

                    outReservations.Add(outReservation);
                }

                return Ok(outReservations);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // GET - Get a specific reservation
        [HttpGet("/accommodations/{accId}/reservations/{resId}")]
        public async Task<ActionResult<OutReservation>> GetReservation([FromRoute] Guid resId)
        {
            try
            {
                var reservation = await _reservationService.GetReservation(resId);

                var outReservation = new OutReservation
                {
                    Id = reservation.Id,
                    NumberOfCompanions = reservation.NumberOfCompanions,
                    IsCompanionAChild = reservation.IsCompanionAChild,
                    NumberOfDaysStaying = reservation.NumberOfDaysStaying,
                    ArrivalDateTime = reservation.ArrivalDateTime,
                    DepartureDateTime = reservation.DepartureDateTime,
                    TotalPrice = reservation.TotalPrice,
                    AccommodationId = reservation.AccommodationId,
                    GuestEmail = reservation.GuestEmail,
                };

                if (reservation.UserId != null)
                {
                    outReservation.UserId = reservation.UserId;
                }

                return Ok(outReservation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // PUT - Update a specific reservation
        [HttpPut("/accommodations/{accId}/reservations/{resId}"), Authorize]
        public async Task<ActionResult<OutReservation>>? UpdateReservation([FromRoute] Guid resId, InReservationDto request)
        {
            try
            {
                var reservation = await _reservationService.UpdateReservation(resId, request);

                var outReservation = new OutReservation
                {
                    Id = reservation.Id,
                    NumberOfCompanions = reservation.NumberOfCompanions,
                    IsCompanionAChild = reservation.IsCompanionAChild,
                    NumberOfDaysStaying = reservation.NumberOfDaysStaying,
                    ArrivalDateTime = reservation.ArrivalDateTime,
                    DepartureDateTime = reservation.DepartureDateTime,
                    TotalPrice = reservation.TotalPrice,
                    AccommodationId = reservation.AccommodationId,
                    GuestEmail = reservation.GuestEmail,
                };

                if (reservation.UserId != null)
                {
                    outReservation.UserId = reservation.UserId;
                }

                return Ok(outReservation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // DELETE - Delete a specific accommodation
        [HttpDelete("/accommodations/{accId}/reservations/{resId}"), Authorize]
        public async Task<ActionResult<string>>? DeleteReservation([FromRoute] Guid resId)
        {
            try
            {
                var result = await _reservationService.DeleteReservation(resId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(204, new { Message = e.Message });
            }
        }

        // REVIEWS

        // POST - Add new review
        [HttpPost("/accommodations/{id}/reviews"), Authorize]
        public async Task<ActionResult<Review>> AddReview([FromRoute] Guid id, InReviewDto request)
        {
            try
            {
                var review = await _reviewService.AddReview(id, request);

                var outReview = new OutReview()
                {
                    Id = review.Id,
                    Comment = review.Comment,
                    CommentDate = review.CommentDateTime.ToShortDateString(),
                    Grade = review.Grade,
                    AccommodationId = review.AccommodationId,
                    UserId = review.UserId,
                    Reviewer = review?.User?.FirstName + " " + review?.User?.LastName,
                };

                return Ok(outReview);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // GET - Get all reviews
        [HttpGet("/accommodations/{id}/reviews")]
        public async Task<ActionResult<List<Review>>> GetReviews([FromRoute] Guid id)
        {
            try
            {
                var reviews = await _reviewService.GetReviews(id);

                var outReviews = new List<OutReview>();

                foreach (var review in reviews)
                {
                    var outReview = new OutReview()
                    {
                        Id = review.Id,
                        Comment = review.Comment,
                        CommentDate = review.CommentDateTime.ToShortDateString(),
                        Grade = review.Grade,
                        AccommodationId = review.AccommodationId,
                        UserId = review.UserId,
                        Reviewer = review?.User?.FirstName + " " + review?.User?.LastName,
                    };

                    outReviews.Add(outReview);
                }

                return Ok(outReviews);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // GET - Get a specific review
        [HttpGet("/accommodations/{accId}/reviews/{revId}")]
        public async Task<ActionResult<Review>> GetReview([FromRoute] Guid revId)
        {
            try
            {
                var review = await _reviewService.GetReview(revId);

                var outReview = new OutReview()
                {
                    Id = review.Id,
                    Comment = review.Comment,
                    CommentDate = review.CommentDateTime.ToShortDateString(),
                    Grade = review.Grade,
                    AccommodationId = review.AccommodationId,
                    UserId = review.UserId,
                    Reviewer = review?.User?.FirstName + " " + review?.User?.LastName,
                };

                return Ok(outReview);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // PUT - Update a specific review
        [HttpPut("/accommodations/{accId}/reviews/{revId}"), Authorize]
        public async Task<ActionResult<OutReview>> UpdateReview([FromRoute] Guid revId, InReviewDto request)
        {
            try
            {
                var review = await _reviewService.UpdateReview(revId, request);

                var outReview = new OutReview()
                {
                    Id = review.Id,
                    Comment = review.Comment,
                    CommentDate = review.CommentDateTime.ToShortDateString(),
                    Grade = review.Grade,
                    AccommodationId = review.AccommodationId,
                    UserId = review.UserId,
                };

                return Ok(outReview);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // DELETE - Delete a specific review
        [HttpDelete("/accommodations/{accId}/reviews/{revId}"), Authorize]
        public async Task<ActionResult<string>>? DeleteReview([FromRoute] Guid revId)
        {
            try
            {
                var result = await _reviewService.DeleteReview(revId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(204, new { Message = e.Message });
            }
        }
    }
}
