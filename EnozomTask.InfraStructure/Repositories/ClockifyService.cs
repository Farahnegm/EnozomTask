
using EnozomTask.InfraStructure.persistence;

using EnozomTask.Domain.Repositories;

using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;



namespace EnozomTask.InfraStructure.Repositories
{
    public class ClockifyService : IClockifyService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _workspaceId;

        public ClockifyService(AppDbContext dbContext, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["Clockify:ApiKey"];
            _workspaceId = configuration["Clockify:WorkspaceId"];
        }

        public async Task PushSampleDataAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

            // Push Projects
            var projects = _dbContext.Projects.Where(p => p.ClockifyId == null).ToList();
            foreach (var project in projects)
            {
                var payload = new { name = project.Name };
                var response = await client.PostAsJsonAsync(
                    $"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/projects", payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ClockifyProjectResponse>();
                    project.ClockifyId = result.id;
                    _dbContext.Projects.Update(project);
                }
                // Optionally handle errors/logging
            }
            await _dbContext.SaveChangesAsync();

            // Push Tasks
            var tasks = _dbContext.TaskItems.Where(t => t.ClockifyId == null).ToList();
            foreach (var task in tasks)
            {
                var project = _dbContext.Projects.FirstOrDefault(p => p.Id == task.ProjectId);
                if (project?.ClockifyId == null) continue;
                var payload = new { name = task.Name };
                var response = await client.PostAsJsonAsync(
                    $"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/projects/{project.ClockifyId}/tasks", payload);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ClockifyTaskResponse>();
                    task.ClockifyId = result.id;
                    _dbContext.TaskItems.Update(task);
                }
            }
            await _dbContext.SaveChangesAsync();

            // Push Time Entries
            var timeEntries = _dbContext.TimeEntries.Where(te => te.ClockifyId == null).ToList();
            foreach (var te in timeEntries)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Id == te.UserId);
                var project = _dbContext.Projects.FirstOrDefault(p => p.Id == te.ProjectId);
                var task = _dbContext.TaskItems.FirstOrDefault(t => t.Id == te.TaskItemId);
                if (user == null || project?.ClockifyId == null || task?.ClockifyId == null) continue;
                var payload = new
                {
                    start = te.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    end = te.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    projectId = project.ClockifyId,
                    taskId = task.ClockifyId,
                    description = task.Name
                };
                var response = await client.PostAsJsonAsync(
                    $"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/time-entries", payload);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ClockifyTimeEntryResponse>();
                    te.ClockifyId = result.id;
                    _dbContext.TimeEntries.Update(te);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        private class ClockifyProjectResponse
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        private class ClockifyTaskResponse
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        private class ClockifyTimeEntryResponse
        {
            public string id { get; set; }
        }
    }
}