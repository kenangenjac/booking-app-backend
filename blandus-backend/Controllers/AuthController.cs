using blandus_backend.Interfaces;
using blandus_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace blandus_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// DI for injecting authService
        /// </summary>
        /// <param name="authService"></param>;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Method to register a user and create a passwordHash and passwordSalt
        /// </summary>
        /// <param name="request"></param>
        /// <returns>User object</returns>
        [HttpPost("/register")]
        public async Task<ActionResult<UserOutModel>> Register(UserRegisterDto request)
        {
            try{
                var user = await _authService.Register(request);
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

                return Ok(outUser);
            } catch(Exception ex) {
                return StatusCode(409, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Login method that returns a JWT if login is successful
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string value</returns>
        [HttpPost("/login")]
        public async Task<ActionResult<UserOutModel>> Login(UserLoginDto request)
        {
            try{
                return Ok(await _authService.Login(request));
                
            } catch(Exception ex) {
                return StatusCode(409, new {Message = ex.Message });
            }
        }
    }
}
