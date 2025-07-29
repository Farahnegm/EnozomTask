using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EnozomTask.Application.DTOs;
using EnozomTask.Application.Services;
using EnozomTask.InfraStructure.persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EnozomTask.InfraStructure.Repositories
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _workspaceId;
        public ReportService(AppDbContext dbContext, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["Clockify:ApiKey"];
            _workspaceId = configuration["Clockify:WorkspaceId"];
        }

        public async Task<List<ReportRowDto>> GetReportRowsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

            var entries = await _dbContext.TimeEntries
                .Include(te => te.User)
                .Include(te => te.Project)
                .Include(te => te.TaskItem)
                .ToListAsync();

            var reportRows = new List<ReportRowDto>();
            foreach (var te in entries)
            {
                double? clockifyTimeSpent = null;
                if (!string.IsNullOrEmpty(te.ClockifyId))
                {
                    var response = await client.GetAsync($"https://api.clockify.me/api/v1/workspaces/{_workspaceId}/time-entries/{te.ClockifyId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var clockifyEntry = await response.Content.ReadFromJsonAsync<ClockifyTimeEntryDto>();
                        if (clockifyEntry != null && clockifyEntry.timeInterval != null)
                        {
                            if (double.TryParse(clockifyEntry.timeInterval.durationHours, NumberStyles.Any, CultureInfo.InvariantCulture, out var hours))
                                clockifyTimeSpent = hours;
                        }
                    }
                }
                reportRows.Add(new ReportRowDto
                {
                    User = te.User.FullName,
                    Project = te.Project.Name,
                    Task = te.TaskItem.Name,
                    LocalEstimate = te.TaskItem.EstimateHours,
                    LocalTimeSpent = (te.EndTime - te.StartTime).TotalHours,
                    ClockifyTimeSpent = clockifyTimeSpent
                });
            }
            return reportRows;
        }

        private class ClockifyTimeEntryDto
        {
            public string id { get; set; }
            public TimeIntervalDto timeInterval { get; set; }
        }
        private class TimeIntervalDto
        {
            public string durationHours { get; set; }
        }
    }
} 