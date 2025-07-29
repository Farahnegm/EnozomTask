using System.Net.Http;
using System.Net.Http.Json;
using EnozomTask.Domain.Interfaces.Strategies;
using EnozomTask.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace EnozomTask.InfraStructure.Strategies
{
    public class ClockifySyncStrategy : ISyncStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _workspaceId;

        public string ProviderName => "Clockify";

        public ClockifySyncStrategy(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
            var assigneeId = taskItem.AssignedUser?.ClockifyId;
            if (string.IsNullOrEmpty(projectClockifyId)) return null;
            
            var nameWithAssignee = !string.IsNullOrEmpty(assignedUserName)
                ? $"{taskItem.Name} (Assigned to: {assignedUserName})"
                : taskItem.Name;
                
            bool isValidClockifyId = !string.IsNullOrEmpty(assigneeId) && 
                                   assigneeId.Length == 24 && 
                                   System.Text.RegularExpressions.Regex.IsMatch(assigneeId, "^[0-9a-fA-F]{24}$");
            
            if (!string.IsNullOrEmpty(assigneeId) && !isValidClockifyId)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid ClockifyId format: {assigneeId} (Length: {assigneeId?.Length})");
            }
                
            object payload;
            if (isValidClockifyId)
            {
                payload = new
                {
                    name = nameWithAssignee,
                    assigneeIds = new[] { assigneeId }
                };
            }
            else
            {
                payload = new
                {
                    name = nameWithAssignee
                };
            }
            
            var response = await _httpClient.PostAsJsonAsync($"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/projects/{projectClockifyId}/tasks", payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Clockify API error: {response.StatusCode} - {errorContent}");
            }
            
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

        public async Task<bool> AssignUsersToProjectAsync(string projectClockifyId, List<string> userClockifyIds)
        {
            if (string.IsNullOrEmpty(projectClockifyId) || !userClockifyIds.Any())
                return false;
                
            try
            {
                var workspaceUsers = await GetUsersAsync();
                var validUserIds = workspaceUsers.Select(u => u.Id).ToList();
                var invalidUserIds = userClockifyIds.Where(id => !validUserIds.Contains(id)).ToList();
                
                if (invalidUserIds.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"Invalid user IDs: {string.Join(", ", invalidUserIds)}");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error assigning users to project: {ex.Message}");
                return false;
            }
        }

        public async Task<List<ExternalUser>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/users");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<ClockifyUserResponse>>();
            return result?.Select(u => new ExternalUser 
            { 
                Id = u.id, 
                Name = u.name, 
                Email = u.email 
            }).ToList() ?? new List<ExternalUser>();
        }
        
        private class ClockifyProjectResponse { public string id { get; set; } }
        private class ClockifyTaskResponse { public string id { get; set; } }
        private class ClockifyTimeEntryResponse { public string id { get; set; } }
        private class ClockifyUserResponse { public string id { get; set; } public string name { get; set; } public string email { get; set; } }
    }
} 