using System.Threading.Tasks;
using EnozomTask.Domain.Entities;

namespace EnozomTask.Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> GetByNameAsync(string name);
        void Add(Project project);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project> GetByIdAsync(int id);
        void Update(Project project);
        void Delete(Project project);
    }
} 