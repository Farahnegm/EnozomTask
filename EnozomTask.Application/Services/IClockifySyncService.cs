using System.Threading.Tasks;
using EnozomTask.Domain.Entities;

namespace EnozomTask.Domain.Repositories
{
    public interface IClockifySyncService
    {
        Task<string> SyncProjectAsync(Project project);
        Task<string> SyncTaskItemAsync(TaskItem taskItem);
        Task<string> SyncTimeEntryAsync(TimeEntry timeEntry);
    }
} 