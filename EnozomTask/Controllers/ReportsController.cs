using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Text;
using EnozomTask.InfraStructure.persistence;
using EnozomTask.Application.DTOs;
using EnozomTask.Application.Interfaces.Services;

namespace EnozomTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var entries = await _reportService.GetReportRowsAsync();
            var sb = new StringBuilder();
            sb.AppendLine("User,Project,Task,Local Estimate,Local Time Spent");
            foreach (var e in entries)
            {
                sb.AppendLine($"{e.User},{e.Project},{e.Task},{e.LocalEstimate},{e.LocalTimeSpent},{e.ClockifyTimeSpent}");
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "report.csv");
        }
    }
} 