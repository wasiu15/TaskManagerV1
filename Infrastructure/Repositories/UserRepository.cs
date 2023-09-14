using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext context): base(context)
        {

        }
        public void CreateUser(User user) => Create(user);
        public void UpdateUser(User user) => Update(user);
        public void DeleteUser(User user) => Delete(user);
        public async Task<List<User>> GetUsers() => await FindAll(false).ToListAsync();
        public async Task<User> GetByUserId(Guid userId, bool trackChanges) => await FindByCondition(x => x.UserId.Equals(userId), trackChanges).FirstOrDefaultAsync();
        public async Task<User> GetByEmail(string email, bool trackChanges) => await FindByCondition(x => x.Email.Equals(email), trackChanges).FirstOrDefaultAsync();
        public async Task<User> GetByEmailAndPassword(string email, string password, bool trackChanges) => await FindByCondition(x => x.Email.Equals(email) && x.Password.Equals(password), trackChanges).FirstOrDefaultAsync();
    }
}
