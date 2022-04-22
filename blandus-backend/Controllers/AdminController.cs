using blandus_backend.Interfaces;
using blandus_backend.Models;
using blandus_backend.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace blandus_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("/users"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserOutModel>> AddUser(UserRegisterDto request)
        {
            try
            {
                var user = await _adminService.AddUser(request);

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
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { Message = ex.Message });
            }
        }

        [HttpPost("/users/admin"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserOutModel>> AddAdmin(UserRegisterDto request)
        {
            try
            {
                var admin = await _adminService.AddAdmin(request);

                var outUser = new UserOutModel()
                {
                    Id = admin.Id,
                    Username = admin.Username,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    PhoneNumber = admin.PhoneNumber,
                    Role = admin.Role,
                };

                return Ok(outUser);
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { Message = ex.Message });
            }
        }

        [HttpGet("/users"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginationUserResponse>> GetUsers([FromQuery] string? search, [FromQuery] int page, [FromQuery] int pageResults, [FromQuery] string? sort)
        {
            try
            {
                var users = await _adminService.GetUsers(search, page, pageResults, sort);
                if (!users.Users.Any())
                {
                    return NotFound();
                }
                return Ok(users);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        [HttpGet("/users/{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserOutModel>> GetUser(Guid id)
        {
            try
            {
                var user = await _adminService.GetUser(id);

                var outUser = new UserOutModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Accommodations = user.Accommodations,
                    Reservations = user.Reservations,
                    Reviews = user.Reviews,
                };

                return Ok(outUser);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        [HttpPut("/users/{id}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<UserOutModel>>? UpdateUser([FromRoute] Guid id, UserUpdateDto request)
        {
            try
            {
                var user = await _adminService.UpdateUser(id, request);

                var outUser = new UserOutModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Accommodations = user.Accommodations,
                    Reservations = user.Reservations,
                    Reviews = user.Reviews,
                };

                return Ok(outUser);
            }
            catch (Exception e)
            {
                return StatusCode(409, new { Message = e.Message });
            }
        }

        [HttpDelete("/users/{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult>? DeleteUser([FromRoute] Guid id)
        {
            try
            {
                var result = await _adminService.DeleteUser(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(204, new { Message = e.Message });
            }
        }
    }
}
