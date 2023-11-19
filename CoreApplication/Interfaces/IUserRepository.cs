using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> Create(User user, string password);
        Task<IEnumerable<string>> GetUnverifiedUsers();

        Task<bool> OnboardUser(string username);
    }
}
