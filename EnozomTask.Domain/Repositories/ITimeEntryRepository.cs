using EnozomTask.Domain.Entities;

namespace EnozomTask.Domain.Repositories
{
    public interface ITimeEntryRepository
    {
        void Add(TimeEntry timeEntry);
        Task<IEnumerable<TimeEntry>> GetAllAsync();
        Task<TimeEntry> GetByIdAsync(int id);
        void Update(TimeEntry timeEntry);
        void Delete(TimeEntry timeEntry);
    }
} 