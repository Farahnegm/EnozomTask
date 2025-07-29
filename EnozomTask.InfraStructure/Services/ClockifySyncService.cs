using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace EnozomTask.InfraStructure.Services
{
    public class ClockifySyncService : IClockifySyncService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _workspaceId;
        public ClockifySyncService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["Clockify:ApiKey"];
            _workspaceId = configuration["Clockify:WorkspaceId"];
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        }
        public async Task<string> SyncProjectAsync(Project project)
        {
            var payload = new { name = project.Name };
            var response = await _httpClient.PostAsJsonAsync($"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/projects", payload);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ClockifyProjectResponse>();
            return result.id;
        }
        public async Task<string> SyncTaskItemAsync(TaskItem taskItem)
        {
            var projectClockifyId = taskItem.Project?.ClockifyId;
            var assignedUserName = taskItem.AssignedUser?.FullName;
            if (string.IsNullOrEmpty(projectClockifyId)) return null;
            var nameWithAssignee = !string.IsNullOrEmpty(assignedUserName)
                ? $"{taskItem.Name} (Assigned to: {assignedUserName})"
                : taskItem.Name;
            var payload = new { name = nameWithAssignee };
            var response = await _httpClient.PostAsJsonAsync($"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/projects/{projectClockifyId}/tasks", payload);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ClockifyTaskResponse>();
            return result.id;
        }
        public async Task<string> SyncTimeEntryAsync(TimeEntry timeEntry)
        {
            var projectClockifyId = timeEntry.Project?.ClockifyId;
            var taskClockifyId = timeEntry.TaskItem?.ClockifyId;
            if (string.IsNullOrEmpty(projectClockifyId) || string.IsNullOrEmpty(taskClockifyId)) return null;
            var payload = new {
                start = timeEntry.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                end = timeEntry.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                projectId = projectClockifyId,
                taskId = taskClockifyId,
                description = timeEntry.TaskItem?.Name
            };
            var response = await _httpClient.PostAsJsonAsync($"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/time-entries", payload);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ClockifyTimeEntryResponse>();
            return result.id;
        }
        private class ClockifyProjectResponse { public string id { get; set; } }
        private class ClockifyTaskResponse { public string id { get; set; } }
        private class ClockifyTimeEntryResponse { public string id { get; set; } }
    }
} 