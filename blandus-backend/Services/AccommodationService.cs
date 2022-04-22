using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using blandus_backend.Interfaces;
using blandus_backend.Models.Accommodation;
using blandus_backend.Models.Image;
using blandus_backend.Models.Service;

namespace blandus_backend.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccommodationService(DataContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<Accommodation> AddAccommodation(InAccommodationDto request)
        {
            if (_contextAccessor.HttpContext == null)
            {
                throw new Exception("You must be logged in to add an accommodation!");
            }

            var username = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            var accommodation = new Accommodation()
            {
                Name = request.Name,
                City = request.City,
                Address = request.Address,
                RoomType = request.RoomType,
                Description = request.Description,
                Contact = request.Contact,
                Type = request.Type,
                Price = request.Price,
                StarRating = request.StarRating,
                User = user,
                UserId = user.Id,
            };

            foreach (var serviceString in request.ServicesArray)
            {
                var service = new Service()
                {
                    Name = serviceString,
                };
                _context.Services.Add(service);
                accommodation.Services?.Add(service);
            }

            foreach (var imageString in request.ImagesArray)
            {
                var image = new Image()
                {
                    Url = imageString,
                };
                _context.Images.Add(image);
                accommodation.Images?.Add(image);
            }

            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            return accommodation;
        }

        public async Task<PaginationAccommodationResponse> GetAccommodations(string? search, AccommodationFilter? filter, int page, int pageResults, string? sort)
        {
            int numberOfAccommodations;
            if (!string.IsNullOrEmpty(search) && (!string.IsNullOrEmpty(filter?.Type) ||
                                                  !string.IsNullOrEmpty(filter?.Service) || filter.Rating.HasValue))
            {
                numberOfAccommodations = (await FindAccommodationsBySearchText(search, filter)).Count;
            }
            else if(!string.IsNullOrEmpty(search))
            {
                numberOfAccommodations = (await FindAccommodationsBySearchText(search, null)).Count;
            }
            else if (!string.IsNullOrEmpty(filter?.Type) ||
                     !string.IsNullOrEmpty(filter?.Service) || filter.Rating.HasValue)
            {
                numberOfAccommodations = (await FindAccommodationsBySearchText(null, filter)).Count;
            }
            else
            {
                numberOfAccommodations = _context.Accommodations.Count();
            }

            pageResults = pageResults < 1 ? numberOfAccommodations : pageResults;
            pageResults = pageResults > numberOfAccommodations ? numberOfAccommodations : pageResults;

            var pageCount = numberOfAccommodations == 0 ? 1 : Math.Ceiling(numberOfAccommodations / (float)pageResults);
            page = page < 1 ? 1 : page;
            page = page > pageCount ? (int)pageCount : page;

            IQueryable<Accommodation> query = _context.Accommodations;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Name.ToLower().Contains(search.ToLower()) ||
                                         a.Description.ToLower().Contains(search.ToLower()) ||
                                         a.City.ToLower().Contains(search.ToLower()) ||
                                         a.Type.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrEmpty(filter?.Type))
            {
                query = query.Where(a => a.Type.ToLower() == filter.Type.ToLower());
            }

            if (!string.IsNullOrEmpty(filter?.Service))
            {
                query = query.Where(a => a.Services.Any(b=>b.Name == filter.Service));
            }

            if (filter.Rating.HasValue)
            {
                query = query.Where(a => a.Rating >= filter.Rating);
            }

            List<Accommodation> accommodations;
            if (string.IsNullOrEmpty(sort))
            {
                accommodations = await query
                    .Include(u => u.User)
                    .Include(s => s.Services)
                    .Include(i => i.Images)
                    .Include(r => r.Reservations)
                    .Include(rw => rw.Reviews)
                    .Include(d => d.DatesOccupied)
                    .Skip((page - 1) * (int)pageResults)
                    .Take((int)pageResults)
                    .ToListAsync();
            }
            else
            {
                if (sort == "asc")
                {
                    accommodations = await query
                        .Include(u => u.User)
                        .Include(s => s.Services)
                        .Include(i => i.Images)
                        .Include(r => r.Reservations)
                        .Include(rw => rw.Reviews)
                        .Include(d => d.DatesOccupied)
                        .OrderBy(a => a.Price)
                        .Skip((page - 1) * (int)pageResults)
                        .Take((int)pageResults)
                        .ToListAsync();
                }
                else
                {
                    accommodations = await query
                        .Include(u => u.User)
                        .Include(s => s.Services)
                        .Include(i => i.Images)
                        .Include(r => r.Reservations)
                        .Include(rw => rw.Reviews)
                        .Include(d => d.DatesOccupied)
                        .OrderByDescending(a => a.Price)
                        .Skip((page - 1) * (int)pageResults)
                        .Take((int)pageResults)
                        .ToListAsync();
                }
            }

            var outAccommodations = new List<OutAccommodation>();
            foreach (var accommodation in accommodations)
            {
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
                    Images = accommodation.Images,
                    Reviews = accommodation.Reviews,
                    Reservations = accommodation.Reservations,
                    DatesOccupied = accommodation.DatesOccupied,
                };
                outAccommodations.Add(outAccommodation);
            }

            var response = new PaginationAccommodationResponse(outAccommodations, (int)pageCount, page);

            return response ?? throw new Exception("No accommodations found");
        }

        public async Task<Accommodation> GetAccommodation(Guid id)
        {
            var accommodation = await _context.Accommodations
                .Where(a => a.Id == id)
                .Include(u => u.User)
                .Include(s => s.Services)
                .Include(i => i.Images)
                .Include(r => r.Reservations)
                .Include(rw => rw.Reviews)
                .Include(d => d.DatesOccupied)
                .FirstOrDefaultAsync();

            return accommodation ?? throw new Exception("No accommodation found");
        }

        public async Task<Accommodation> UpdateAccommodation(Guid id, InAccommodationDto request)
        {
            var accommodationToUpdate = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == id);
            if (accommodationToUpdate is null) throw new Exception("No accommodation found");

            var username = _contextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (accommodationToUpdate.UserId != user?.Id) throw new Exception("You can only update your own accommodation");

            var services = await _context.Services
                .Where(s => s.AccommodationId == id)
                .ToListAsync();

            var images = await _context.Images
                .Where(i => i.AccommodationId == id)
                .ToListAsync();

            accommodationToUpdate.Name = request.Name;
            accommodationToUpdate.City = request.City;
            accommodationToUpdate.Address = request.Address;
            accommodationToUpdate.RoomType = request.RoomType;
            accommodationToUpdate.Description = request.Description;
            accommodationToUpdate.Contact = request.Contact;
            accommodationToUpdate.Type = request.Type;
            accommodationToUpdate.Price = request.Price;
            accommodationToUpdate.StarRating = request.StarRating;

            int counter = 0;
            foreach (var service in services)
            {
                service.Name = request.ServicesArray[counter];
                counter++;
            }

            counter = 0;
            foreach (var image in images)
            {
                image.Url = request.ImagesArray[counter];
                counter++;
            }

            await _context.SaveChangesAsync();

            return await GetAccommodation(id);
        }

        public async Task<string> DeleteAccommodation(Guid id)
        {
            var accommodationToDelete = await _context.Accommodations.FirstOrDefaultAsync(a => a.Id == id);
            if (accommodationToDelete is null) return "No accommodation with given Id found";

            var username = _contextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (accommodationToDelete.UserId != user?.Id) throw new Exception("You can only delete your own accommodation");

            var servicesToDelete = await _context.Services
                .Where(s => s.AccommodationId == id)
                .ToListAsync();

            var imagesToDelete = await _context.Images
                .Where(i => i.AccommodationId == id)
                .ToListAsync();

            var reservationsToDelete = await _context.Reservations
                .Where(r => r.AccommodationId == id)
                .ToListAsync();

            var datesToDelete = await _context.DatesOccupied
                .Where(d => d.AccommodationId == id)
                .ToListAsync();

            var reviewsToDelete = await _context.Reviews
                .Where(re => re.AccommodationId == id)
                .ToListAsync();

            _context.Accommodations.Remove(accommodationToDelete);
            foreach (var service in servicesToDelete) _context.Services.Remove(service);
            foreach (var image in imagesToDelete) _context.Images.Remove(image);
            foreach (var reservation in reservationsToDelete) _context.Reservations.Remove(reservation);
            foreach (var dateToDelete in datesToDelete) _context.DatesOccupied.Remove(dateToDelete);
            foreach (var review in reviewsToDelete) _context.Reviews.Remove(review);

            await _context.SaveChangesAsync();

            return "Accommodation successfully removed";
        }

        // helper to count searched accommodation
        private async Task<List<Accommodation>> FindAccommodationsBySearchText(string? searchText = null, AccommodationFilter? filter = null)
        {
            if (filter != null && !string.IsNullOrEmpty(searchText) && (!string.IsNullOrEmpty(filter?.Type) ||
                                                                        !string.IsNullOrEmpty(filter?.Service) || filter.Rating.HasValue))
            {
                IQueryable<Accommodation> query = _context.Accommodations
                    .Where(a => a.Name.ToLower().Contains(searchText.ToLower()) ||
                                a.Description.ToLower().Contains(searchText.ToLower()) ||
                                a.City.ToLower().Contains(searchText.ToLower()) ||
                                a.Type.ToLower().Contains(searchText.ToLower()));

                if (!string.IsNullOrEmpty(filter?.Type))
                {
                    query = query.Where(a => a.Type.ToLower() == filter.Type.ToLower());
                }

                if (filter.Rating.HasValue)
                {
                    query = query.Where(a => a.Rating >= filter.Rating);
                }

                if (!string.IsNullOrEmpty(filter?.Service))
                {
                    query = query.Where(a => a.Services.Any(b => b.Name == filter.Service));
                }

                return await query
                    .Include(u => u.User)
                    .Include(s => s.Services)
                    .Include(i => i.Images)
                    .Include(r => r.Reviews)
                    .ToListAsync();
            }
            

            if (filter != null && (!string.IsNullOrEmpty(filter?.Type) ||
                                   !string.IsNullOrEmpty(filter?.Service) || filter.Rating.HasValue))
            {
                IQueryable<Accommodation> query = _context.Accommodations;

                if (!string.IsNullOrEmpty(filter?.Type))
                {
                    query = query.Where(a => a.Type.ToLower() == filter.Type.ToLower());
                }

                if (filter.Rating.HasValue)
                {
                    query = query.Where(a => a.Rating >= filter.Rating);
                }

                if (!string.IsNullOrEmpty(filter?.Service))
                {
                    query = query.Where(a => a.Services.Any(b => b.Name == filter.Service));
                }

                return await query
                    .Include(u => u.User)
                    .Include(s => s.Services)
                    .Include(i => i.Images)
                    .Include(r => r.Reviews)
                    .ToListAsync();
            }

            return await _context.Accommodations
                .Where(a => a.Name.ToLower().Contains(searchText.ToLower()) ||
                            a.Description.ToLower().Contains(searchText.ToLower()) ||
                            a.City.ToLower().Contains(searchText.ToLower()) ||
                            a.Type.ToLower().Contains(searchText.ToLower()))
                .Include(u => u.User)
                .Include(s => s.Services)
                .Include(i => i.Images)
                .Include(r => r.Reviews)
                .ToListAsync();
        }
    }
}
