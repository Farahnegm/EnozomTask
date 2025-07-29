using System.Threading.Tasks;
using EnozomTask.Application.DTOs;

namespace EnozomTask.Application.Interfaces.Services
{
    public interface ITimeEntryOrchestrationService
    {
        Task HandleTimeEntryAsync(TimeEntryCreateDto dto);
    }
} 