using EnozomTask.Application.DTOs;

namespace EnozomTask.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<object> CreateTaskAsync(TaskItemCreateDto dto);
        Task<IEnumerable<TaskItemReadDto>> GetAllTasksAsync();
        Task<TaskItemReadDto> GetTaskByIdAsync(int id);
        Task<TaskItemReadDto> UpdateTaskAsync(int id, TaskItemUpdateDto dto);
        Task DeleteTaskAsync(int id);
    }
} 