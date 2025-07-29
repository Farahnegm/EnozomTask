using EnozomTask.Domain.Entities;
using EnozomTask.InfraStructure.persistence;
using Restaurants.Infrastructure.Seeders;


namespace EnozomTask.InfraStructure.Seeders
{ 
    public class DataSeeder : IDataSeeder
    {
        private readonly AppDbContext _dbContext;
        public DataSeeder(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            if (!_dbContext.Users.Any())
            {
                var users = new List<User>
                {
                    new User { FullName = "Alice Johnson" },
                    new User { FullName = "Bob Martinez" },
                    new User { FullName = "Carla Nguyen" },
                    new User { FullName = "David Lee" },
                    new User { FullName = "Emma Smith" }
                };
                _dbContext.Users.AddRange(users);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.Projects.Any())
            {
                var projects = new List<Project>
                {
                    new Project { Name = "Website Redesign" },
                    new Project { Name = "Mobile App Launch" },
                    new Project { Name = "Marketing Campaign" }
                };
                _dbContext.Projects.AddRange(projects);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.TaskItems.Any())
            {
                var websiteRedesign = _dbContext.Projects.First(p => p.Name == "Website Redesign");
                var mobileAppLaunch = _dbContext.Projects.First(p => p.Name == "Mobile App Launch");
                var marketingCampaign = _dbContext.Projects.First(p => p.Name == "Marketing Campaign");

                var tasks = new List<TaskItem>
                {
                    new TaskItem { Name = "Homepage Mockup", ProjectId = websiteRedesign.Id, EstimateHours = 7 },
                    new TaskItem { Name = "Content Migration", ProjectId = websiteRedesign.Id, EstimateHours = 10 },
                    new TaskItem { Name = "SEO Optimization", ProjectId = websiteRedesign.Id, EstimateHours = 12 },
                    new TaskItem { Name = "API Integration", ProjectId = mobileAppLaunch.Id, EstimateHours = 5 },
                    new TaskItem { Name = "User Testing", ProjectId = mobileAppLaunch.Id, EstimateHours = 3 },
                    new TaskItem { Name = "Bug Fixing", ProjectId = mobileAppLaunch.Id, EstimateHours = 8 },
                    new TaskItem { Name = "Ad Design", ProjectId = marketingCampaign.Id, EstimateHours = 9 },
                    new TaskItem { Name = "Social Media Scheduling", ProjectId = marketingCampaign.Id, EstimateHours = 14 },
                    new TaskItem { Name = "Email Outreach", ProjectId = marketingCampaign.Id, EstimateHours = 24 }
                };
                _dbContext.TaskItems.AddRange(tasks);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.TimeEntries.Any())
            {
                var users = _dbContext.Users.ToList();
                var projects = _dbContext.Projects.ToList();
                var tasks = _dbContext.TaskItems.ToList();
                var timeEntries = new List<TimeEntry>
                {
                    new TimeEntry { UserId = users.First(u => u.FullName == "Alice Johnson").Id, ProjectId = projects.First(p => p.Name == "Website Redesign").Id, TaskItemId = tasks.First(t => t.Name == "Homepage Mockup").Id, StartTime = DateTime.Parse("2025-07-20T09:15:00Z"), EndTime = DateTime.Parse("2025-07-20T11:45:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "Bob Martinez").Id, ProjectId = projects.First(p => p.Name == "Website Redesign").Id, TaskItemId = tasks.First(t => t.Name == "Content Migration").Id, StartTime = DateTime.Parse("2025-07-21T13:00:00Z"), EndTime = DateTime.Parse("2025-07-21T15:20:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "Carla Nguyen").Id, ProjectId = projects.First(p => p.Name == "Website Redesign").Id, TaskItemId = tasks.First(t => t.Name == "SEO Optimization").Id, StartTime = DateTime.Parse("2025-07-22T10:05:00Z"), EndTime = DateTime.Parse("2025-07-22T12:50:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "David Lee").Id, ProjectId = projects.First(p => p.Name == "Mobile App Launch").Id, TaskItemId = tasks.First(t => t.Name == "API Integration").Id, StartTime = DateTime.Parse("2025-07-23T11:10:00Z"), EndTime = DateTime.Parse("2025-07-23T13:35:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "Emma Smith").Id, ProjectId = projects.First(p => p.Name == "Mobile App Launch").Id, TaskItemId = tasks.First(t => t.Name == "User Testing").Id, StartTime = DateTime.Parse("2025-07-24T14:05:00Z"), EndTime = DateTime.Parse("2025-07-24T16:35:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "Alice Johnson").Id, ProjectId = projects.First(p => p.Name == "Mobile App Launch").Id, TaskItemId = tasks.First(t => t.Name == "Bug Fixing").Id, StartTime = DateTime.Parse("2025-07-25T09:15:00Z"), EndTime = DateTime.Parse("2025-07-25T11:45:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "Bob Martinez").Id, ProjectId = projects.First(p => p.Name == "Marketing Campaign").Id, TaskItemId = tasks.First(t => t.Name == "Ad Design").Id, StartTime = DateTime.Parse("2025-07-26T10:10:00Z"), EndTime = DateTime.Parse("2025-07-26T12:10:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "Carla Nguyen").Id, ProjectId = projects.First(p => p.Name == "Marketing Campaign").Id, TaskItemId = tasks.First(t => t.Name == "Social Media Scheduling").Id, StartTime = DateTime.Parse("2025-07-27T09:05:00Z"), EndTime = DateTime.Parse("2025-07-27T10:35:00Z") },
                    new TimeEntry { UserId = users.First(u => u.FullName == "David Lee").Id, ProjectId = projects.First(p => p.Name == "Marketing Campaign").Id, TaskItemId = tasks.First(t => t.Name == "Email Outreach").Id, StartTime = DateTime.Parse("2025-07-28T15:10:00Z"), EndTime = DateTime.Parse("2025-07-28T17:05:00Z") }
                };
                _dbContext.TimeEntries.AddRange(timeEntries);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
} 