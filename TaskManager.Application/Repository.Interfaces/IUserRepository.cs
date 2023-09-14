using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface IUserRepository
    {
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        Task<User> GetByUserId(Guid userId, bool trackChanges);
        Task<User> GetByEmail(string email, bool trackChanges);
        Task<List<User>> GetUsers();
    }
}
