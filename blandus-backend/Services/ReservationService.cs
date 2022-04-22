using System.Security.Claims;
using blandus_backend.Interfaces;
using blandus_backend.Models.Accommodation;
using blandus_backend.Models.DatesOccupied;
using blandus_backend.Models.Reservation;

namespace blandus_backend.Services
{
    public class ReservationService : IReservationService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ReservationService(DataContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<Reservation> AddReservation(Guid accommodationId, InReservationDto request)
        {
            var accommodation = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == accommodationId);

            if (accommodation is null)
            {
                throw new Exception("Accommodation not found");
            }

            var periodOccupied = new DatesOccupied()
            {
                ArrivalDateTime = request.ArrivalDateTime,
                DepartureDateTime = request.DepartureDateTime,
                AccommodationId = accommodationId,
                Accommodation = accommodation,
            };

            _context.DatesOccupied.Add(periodOccupied);

            var reservation = new Reservation
            {
                NumberOfCompanions = request.NumberOfCompanions,
                IsCompanionAChild = request.IsCompanionAChild,
                NumberOfDaysStaying = request.NumberOfDaysStaying,
                ArrivalDateTime = request.ArrivalDateTime,
                DepartureDateTime = request.DepartureDateTime,
                TotalPrice = request.TotalPrice,
                GuestEmail = request.GuestEmail,
                Accommodation = accommodation
            };


            var username = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(username)) // if the User is making a reservation
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                reservation.User = user;
                user?.Reservations?.Add(reservation);
            }
            else    // if the Guest is making a reservation
            {
                reservation.GuestEmail = request.GuestEmail;
            }

            _context.Reservations.Add(reservation);
            accommodation.Reservations?.Add(reservation);
            accommodation.DatesOccupied?.Add(periodOccupied);

            await _context.SaveChangesAsync();

            return reservation;
        }

        public async Task<List<Reservation>> GetReservations(Guid accommodationId)
        {
            var reservations = await _context.Reservations
                .Where(a => a.AccommodationId == accommodationId)
                .Include(u => u.User)
                .Include(a => a.Accommodation)
                .ToListAsync();

            return reservations ?? throw new Exception("No reservations found"); ;
        }

        public async Task<Reservation> GetReservation(Guid reservationId)
        {
            var reservation = await _context.Reservations
                .Include(u => u.User)
                .Include(a => a.Accommodation)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            return reservation ?? throw new Exception("No reservations found"); ;
        }

        public async Task<Reservation> UpdateReservation(Guid resId, InReservationDto request)
        {
            var reservationToUpdate = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == resId);
            if (reservationToUpdate == null) throw new Exception("No reservations found");


            var username = _contextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(username)) // if the User is updating a reservation
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (reservationToUpdate?.UserId != user?.Id) throw new Exception("You can only update your own reservation");
            }
            else
            {
                if (reservationToUpdate.GuestEmail != request.GuestEmail) throw new Exception("You can only update your own reservation");
            }


            var accommodation =
                await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == reservationToUpdate.AccommodationId);

            var dates = await _context.DatesOccupied
                .Where(d => d.AccommodationId == accommodation.Id)
                .ToListAsync();


            reservationToUpdate.NumberOfCompanions = request.NumberOfCompanions;
            reservationToUpdate.IsCompanionAChild = request.IsCompanionAChild;
            reservationToUpdate.NumberOfDaysStaying = request.NumberOfDaysStaying;
            reservationToUpdate.ArrivalDateTime = request.ArrivalDateTime;
            reservationToUpdate.DepartureDateTime = request.DepartureDateTime;
            reservationToUpdate.TotalPrice = request.TotalPrice;
            reservationToUpdate.GuestEmail = request.GuestEmail;

            bool isDateFound = false;
            for (int i = 0; i < accommodation?.DatesOccupied?.Count; i++)
            {
                for (int j = 0; j < dates.Count; j++)
                {
                    if (accommodation.DatesOccupied[i].Id != dates[i].Id) continue;
                    dates[i].ArrivalDateTime = request.ArrivalDateTime;
                    dates[i].DepartureDateTime = request.DepartureDateTime;
                    isDateFound = true;
                }

                if (isDateFound) break;
            }

            await _context.SaveChangesAsync();

            return await GetReservation(resId);
        }

        public async Task<string> DeleteReservation(Guid reservationId)
        {
            var reservationToDelete = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservationToDelete == null) throw new Exception("No reservations found");

            var username = _contextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(username)) // if the User is updating a reservation
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (reservationToDelete?.UserId != user?.Id) throw new Exception("You can only delete your own reservation");
            }

            var datesRelatedToReservation = await _context.DatesOccupied
                .Where(r => r.AccommodationId == reservationToDelete.AccommodationId)
                .ToListAsync();

            _context.Reservations.Remove(reservationToDelete);
            foreach (var datesOccupied in datesRelatedToReservation) _context.DatesOccupied.Remove(datesOccupied);

            await _context.SaveChangesAsync();

            return "Reservation deleted successfully";
        }
    }
}
