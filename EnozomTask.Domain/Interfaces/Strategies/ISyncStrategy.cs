using EnozomTask.Domain.Entities;

namespace EnozomTask.Domain.Interfaces.Strategies
{
    public interface ISyncStrategy
    {
        Task<string> SyncProjectAsync(Project project);
        Task<string> SyncTaskItemAsync(TaskItem taskItem);
        Task<string> SyncTimeEntryAsync(TimeEntry timeEntry);
        Task<bool> AssignUsersToProjectAsync(string projectId, List<string> userIds);
        Task<List<ExternalUser>> GetUsersAsync();
        string ProviderName { get; }
    }

    public class ExternalUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
} 