using System.Threading.Tasks;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EnozomTask.InfraStructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> GetByFullNameAsync(string fullName)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.FullName == fullName);
        }
        public void Add(User user)
        {
            _dbContext.Users.Add(user);
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }
        public void Update(User user)
        {
            _dbContext.Users.Update(user);
        }
        public void Delete(User user)
        {
            _dbContext.Users.Remove(user);
        }
    }
} 