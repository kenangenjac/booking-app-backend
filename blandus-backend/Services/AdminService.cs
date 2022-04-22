using System.Security.Cryptography;
using System.Text.RegularExpressions;
using blandus_backend.Interfaces;
using blandus_backend.Models;
using blandus_backend.Models.Accommodation;
using blandus_backend.Models.User;

namespace blandus_backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _context;

        public AdminService(DataContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(UserRegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = await _context.Users
                .Where(u => u.Username == request.Username || u.Email == request.Email || u.PhoneNumber == request.PhoneNumber)
                .FirstOrDefaultAsync();

            if (user != null || user?.Username == request.Username || user?.Email == request.Email || user?.PhoneNumber == request.PhoneNumber)
            {
                throw new Exception("Invalid credentials");
            }

            if (request.PhoneNumber.Length < 9 || request.PhoneNumber.Any(x => char.IsLetter(x)))
            {
                throw new Exception("Invalid phone number");
            }

            Regex passwordCheckRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (!passwordCheckRegex.IsMatch(request.Password))
            {
                throw new Exception("Invalid password");
            }

            user = new User
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> AddAdmin(UserRegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = await _context.Users
                .Where(u => u.Username == request.Username || u.Email == request.Email || u.PhoneNumber == request.PhoneNumber)
                .FirstOrDefaultAsync();

            if (user != null || user?.Username == request.Username || user?.Email == request.Email || user?.PhoneNumber == request.PhoneNumber)
            {
                throw new Exception("Invalid credentials");
            }

            if (request.PhoneNumber.Length < 9 || request.PhoneNumber.Any(x => char.IsLetter(x)))
            {
                throw new Exception("Invalid phone number");
            }

            Regex passwordCheckRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (!passwordCheckRegex.IsMatch(request.Password))
            {
                throw new Exception("Invalid password");
            }

            var admin = new User
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "Admin"
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            return admin;
        }

        public async Task<PaginationUserResponse> GetUsers(string? search, int page, int pageResults, string? sort)
        {
            var numberOfAccommodations = !string.IsNullOrEmpty(search)
                ? (await FindUsersBySearchText(search)).Count
                : _context.Accommodations.Count();

            pageResults = pageResults < 1 ? numberOfAccommodations : pageResults;
            pageResults = pageResults > numberOfAccommodations ? numberOfAccommodations : pageResults;

            var pageCount = numberOfAccommodations == 0 ? 1 : Math.Ceiling(numberOfAccommodations / (float)pageResults);
            page = page < 1 ? 1 : page;
            page = page > pageCount ? (int)pageCount : page;

            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.FirstName.ToLower().Contains(search.ToLower()) ||
                                         a.LastName.ToLower().Contains(search.ToLower()) ||
                                         a.Username.ToLower().Contains(search.ToLower()) ||
                                         a.Email.ToLower().Contains(search.ToLower()));

            }

            List<User> users;
            if (string.IsNullOrEmpty(sort))
            {
                users = await query
                    .Include(a => a.Accommodations)
                    .Include(r => r.Reservations)
                    .Include(rw => rw.Reviews)
                    .Skip((page - 1) * (int) pageResults)
                    .Take((int) pageResults)
                    .ToListAsync();
            }
            else
            {
                if (sort == "asc")
                {
                    users = await query
                        .Include(a => a.Accommodations)
                        .Include(r => r.Reservations)
                        .Include(rw => rw.Reviews)
                        .OrderBy(a => a.FirstName)
                        .Skip((page - 1) * (int)pageResults)
                        .Take((int)pageResults)
                        .ToListAsync();
                }
                else
                {
                    users = await query
                        .Include(a => a.Accommodations)
                        .Include(r => r.Reservations)
                        .Include(rw => rw.Reviews)
                        .OrderByDescending(a => a.FirstName)
                        .Skip((page - 1) * (int)pageResults)
                        .Take((int)pageResults)
                        .ToListAsync();
                }
            }

            var outUsers = new List<UserOutModel>();
            foreach (var user in users)
            {
                var outUser = new UserOutModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                };
                outUsers.Add(outUser);
            }

            var response = new PaginationUserResponse(outUsers, (int)pageCount, page);

            return response ?? throw new Exception("No users found");
        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await _context.Users
                    .Where(a => a.Id == id)
                    .Include(a => a.Accommodations)
                    .Include(r => r.Reservations)
                    .Include(rw => rw.Reviews)
                    .FirstOrDefaultAsync();

            return user ?? throw new Exception("No users found");
        }

        public async Task<User> UpdateUser(Guid id, UserUpdateDto request)
        {
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
            if (userToUpdate is null) throw new Exception("No users found");

            var user = await _context.Users
                .Where(u => u.Username == request.Username || u.Email == request.Email || u.PhoneNumber == request.PhoneNumber)
                .FirstOrDefaultAsync();

            if (user != null || user?.Username == request.Username || user?.Email == request.Email || user?.PhoneNumber == request.PhoneNumber)
            {
                throw new Exception("Invalid credentials");
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (request.PhoneNumber.Length < 9 || request.PhoneNumber.Any(x => char.IsLetter(x)))
                {
                    throw new Exception("Invalid phone number");
                }
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                Regex passwordCheckRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
                if (!passwordCheckRegex.IsMatch(request.Password))
                {
                    throw new Exception("Invalid password");
                }
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                userToUpdate.PasswordHash = passwordHash;
                userToUpdate.PasswordSalt = passwordSalt;
            }

            if (!string.IsNullOrEmpty(request.Username)) userToUpdate.Username = request.Username;
            if (!string.IsNullOrEmpty(request.FirstName)) userToUpdate.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) userToUpdate.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Email)) userToUpdate.Email = request.Email;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) userToUpdate.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            return await GetUser(id);
        }

        public async Task<string> DeleteUser(Guid id)
        {
            var userToDelete = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
            if (userToDelete is null) throw new Exception("No users found");

            _context.Users.Remove(userToDelete);  // if user is deleted, his ratings, reservations and accommodations are still in the database
            await _context.SaveChangesAsync();

            return "User deleted successfully";
        }

        private async Task<List<User>> FindUsersBySearchText(string search)
        {
            return await _context.Users
                .Where(a => a.FirstName.ToLower().Contains(search.ToLower()) ||
                            a.LastName.ToLower().Contains(search.ToLower()) ||
                            a.Username.ToLower().Contains(search.ToLower()) ||
                            a.Email.ToLower().Contains(search.ToLower()))
                .Include(a => a.Accommodations)
                .Include(r => r.Reservations)
                .Include(rw => rw.Reviews)
                .ToListAsync();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
