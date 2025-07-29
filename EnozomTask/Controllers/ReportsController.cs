using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Text;
using EnozomTask.InfraStructure.persistence;

namespace EnozomTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public ReportsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("export")]
        public IActionResult Export()
        {
            var entries = from te in _dbContext.TimeEntries
                           join u in _dbContext.Users on te.UserId equals u.Id
                           join p in _dbContext.Projects on te.ProjectId equals p.Id
                           join t in _dbContext.TaskItems on te.TaskItemId equals t.Id
                           select new
                           {
                               User = u.FullName,
                               Project = p.Name,
                               Task = t.Name,
                               LocalEstimate = t.EstimateHours,
                               LocalTimeSpent = (te.EndTime - te.StartTime).TotalHours.ToString("0.00", CultureInfo.InvariantCulture)
                           };

            var sb = new StringBuilder();
            sb.AppendLine("User,Project,Task,Local Estimate,Local Time Spent");
            foreach (var e in entries)
            {
                sb.AppendLine($"{e.User},{e.Project},{e.Task},{e.LocalEstimate},{e.LocalTimeSpent}");
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "report.csv");
        }
    }
} 