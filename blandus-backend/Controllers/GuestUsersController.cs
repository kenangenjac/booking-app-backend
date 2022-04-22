using blandus_backend.Interfaces;
using blandus_backend.Models.Reservation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace blandus_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestUsersController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public GuestUsersController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // POST - Add new reservation
        [HttpPost("/guest/accommodations/{id}/reservations/")]
        public async Task<ActionResult<OutReservation>> AddReservation([FromRoute(Name = "id")] Guid accommodationId, InReservationDto request)
        {
            try
            {
                var reservation = await _reservationService.AddReservation(accommodationId, request);

                var outReservation = new OutReservation()
                {
                    Id = reservation.Id,
                    NumberOfCompanions = reservation.NumberOfCompanions,
                    IsCompanionAChild = reservation.IsCompanionAChild,
                    NumberOfDaysStaying = reservation.NumberOfDaysStaying,
                    ArrivalDateTime = reservation.ArrivalDateTime,
                    DepartureDateTime = reservation.DepartureDateTime,
                    AccommodationId = reservation.AccommodationId,
                    GuestEmail = reservation.GuestEmail,
                };

                return Ok(outReservation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // PUT - Update a specific reservation
        [HttpPut("/guest/accommodations/{accId}/reservations/{resId}")]
        public async Task<ActionResult<OutReservation>> UpdateReservation([FromRoute] Guid resId, InReservationDto request)
        {
            try
            {
                var reservation = await _reservationService.UpdateReservation(resId, request);

                var outReservation = new OutReservation()
                {
                    Id = reservation.Id,
                    NumberOfCompanions = reservation.NumberOfCompanions,
                    IsCompanionAChild = reservation.IsCompanionAChild,
                    NumberOfDaysStaying = reservation.NumberOfDaysStaying,
                    ArrivalDateTime = reservation.ArrivalDateTime,
                    DepartureDateTime = reservation.DepartureDateTime,
                    AccommodationId = reservation.AccommodationId,
                    GuestEmail = reservation.GuestEmail,
                };

                return Ok(outReservation);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        // DELETE - Delete a specific accommodation
        [HttpDelete("/guest/accommodations/{accId}/reservations/{resId}")]
        public async Task<ActionResult<string>> DeleteReservation([FromRoute] Guid resId)
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
    }
}
