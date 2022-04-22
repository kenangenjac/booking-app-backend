using blandus_backend.Models;
using blandus_backend.Models.User;

namespace blandus_backend.Interfaces
{
    public interface IAdminService
    {
        public Task<User> AddUser(UserRegisterDto request);
        public Task<User> AddAdmin(UserRegisterDto request);
        public Task<PaginationUserResponse> GetUsers(string? search, int page, int pageResults, string? sort);
        public Task<User> GetUser(Guid id);
        public Task<User> UpdateUser(Guid id, UserUpdateDto request);
        public Task<string> DeleteUser(Guid id);
    }
}
