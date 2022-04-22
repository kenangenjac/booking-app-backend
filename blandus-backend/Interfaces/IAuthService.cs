using blandus_backend.Models;
using blandus_backend.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace blandus_backend.Interfaces
{
    public interface IAuthService
    {
        public Task<User> Register(UserRegisterDto request);
        public Task<UserOutModel> Login(UserLoginDto request);
    }
}
