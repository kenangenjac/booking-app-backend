using blandus_backend.Models.Accommodation;
using blandus_backend.Models.Reservation;

namespace blandus_backend.Interfaces
{
    public interface IReservationService
    {
        public Task<Reservation> AddReservation(Guid id, InReservationDto request);
        public Task<List<Reservation>> GetReservations(Guid id);
        public Task<Reservation> GetReservation(Guid reservationId);
        public Task<Reservation> UpdateReservation(Guid reservationId, InReservationDto request);
        public Task<string> DeleteReservation(Guid reservationId);
    }
}
