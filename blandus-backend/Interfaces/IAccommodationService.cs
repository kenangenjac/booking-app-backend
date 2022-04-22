using blandus_backend.Models.Accommodation;

namespace blandus_backend.Interfaces
{
    public interface IAccommodationService
    {
        public Task<Accommodation> AddAccommodation(InAccommodationDto request);
        public Task<PaginationAccommodationResponse> GetAccommodations(string? search, AccommodationFilter? filter, int page, int pageResults, string? sort);
        public Task<Accommodation> GetAccommodation(Guid id);
        public Task<Accommodation> UpdateAccommodation(Guid id, InAccommodationDto request);
        public Task<string> DeleteAccommodation(Guid id);
    }
}
