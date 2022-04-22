using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using blandus_backend.Interfaces;
using blandus_backend.Models;
using blandus_backend.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace blandus_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        /// <summary>
        /// DI for injecting configuration to access appsettings.json and for database access
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        public AuthService(IConfiguration configuration, DataContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Method to register a user and create a passwordHash and passwordSalt
        /// </summary>
        /// <param name="request"></param>
        /// <returns>User object</returns>
        [HttpPost("register")]
        public async Task<User> Register(UserRegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = await _context.Users
                .Where(u => u.Username == request.Username || u.Email == request.Email || u.PhoneNumber == request.PhoneNumber)
                .FirstOrDefaultAsync();

            if(user != null)
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

        /// <summary>
        /// Method to create a passwordHash and a passwordSalt for a registered user
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Login method that returns a JWT if login is successful
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string value</returns>
        [HttpPost("login")]
        public async Task<UserOutModel> Login(UserLoginDto request)
        {
            var user = await _context.Users
                .Where(u => u.Username == request.Username)
                .FirstOrDefaultAsync();

            if (user is null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Invalid credentials");
            }

            var token = CreateToken(user);

            return new UserOutModel()
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Token = token,
            };
        }

        /// <summary>
        /// Method that hashes the provided password from login request and checks weather it corresponds to the provided passwordHash of a user
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns>bool value</returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return hash.SequenceEqual(passwordHash);
            }
        }

        /// <summary>
        /// Method to create a JSON Web Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>string value</returns>
        private string CreateToken(User user)
        {
            var tokenClaims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role),
                new(ClaimTypes.Role, user.Role),
            };

            // accessing token key in appsettings.json
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // defining the payload of the jwt
            var token = new JwtSecurityToken(
                claims: tokenClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
