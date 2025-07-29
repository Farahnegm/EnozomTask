using System.Collections.Generic;
using System.Threading.Tasks;
using EnozomTask.Application.DTOs;

namespace EnozomTask.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<List<ReportRowDto>> GetReportRowsAsync();
    }
} 