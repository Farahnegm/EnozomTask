using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;

namespace EnozomTask.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<ProjectReadDto> CreateProjectAsync(ProjectCreateDto dto);
        Task<IEnumerable<ProjectReadDto>> GetAllProjectsAsync();
        Task<ProjectReadDto> GetProjectByIdAsync(int id);
        Task<ProjectReadDto> UpdateProjectAsync(int id, ProjectUpdateDto dto);
        Task DeleteProjectAsync(int id);
        Task<object> AssignUsersToProjectAsync(int projectId, List<int> userIds);
    }
} 