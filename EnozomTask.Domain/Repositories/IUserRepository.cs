using System.Threading.Tasks;
using EnozomTask.Domain.Entities;

namespace EnozomTask.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByFullNameAsync(string fullName);
        void Add(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        void Update(User user);
        void Delete(User user);
    }
} 