using System.Threading.Tasks;
using EnozomTask.Domain.Entities;
using System.Collections.Generic;

namespace EnozomTask.Application.Interfaces.Services
{
    public interface IClockifySyncService
    {
        Task<string> SyncProjectAsync(Project project);
        Task<string> SyncTaskItemAsync(TaskItem taskItem);
        Task<string> SyncTimeEntryAsync(TimeEntry timeEntry);
        Task<List<ClockifyUser>> GetClockifyUsersAsync();
        Task<bool> AssignUsersToProjectAsync(string projectClockifyId, List<string> userClockifyIds);
    }
    
    public class ClockifyUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
} 