using System.Threading.Tasks;
using EnozomTask.Domain.Entities;

namespace EnozomTask.Domain.Repositories
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> GetByNameAndProjectIdAsync(string name, int projectId);
        void Add(TaskItem taskItem);
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem> GetByIdAsync(int id);
        void Update(TaskItem taskItem);
        void Delete(TaskItem taskItem);
    }
} 