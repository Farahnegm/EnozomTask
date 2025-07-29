using System.Threading.Tasks;
using EnozomTask.Application.DTOs;
using EnozomTask.Application.Interfaces.Services;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;

namespace EnozomTask.InfraStructure.Services
{
    public class TimeEntryOrchestrationService : ITimeEntryOrchestrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TimeEntryOrchestrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleTimeEntryAsync(TimeEntryCreateDto dto)
        {
            var user = await _unitOfWork.Users.GetByFullNameAsync(dto.UserFullName);
            if (user == null)
            {
                user = new User { FullName = dto.UserFullName };
                _unitOfWork.Users.Add(user);
            }

            var project = await _unitOfWork.Projects.GetByNameAsync(dto.ProjectName);
            if (project == null)
            {
                project = new Project { Name = dto.ProjectName };
                _unitOfWork.Projects.Add(project);
            }

            var task = await _unitOfWork.TaskItems.GetByNameAndProjectIdAsync(dto.TaskName, project?.Id ?? 0);
            if (task == null)
            {
                task = new TaskItem { Name = dto.TaskName, Project = project, EstimateHours = dto.EstimateHours };
                _unitOfWork.TaskItems.Add(task);
            }

            var timeEntry = new TimeEntry
            {
                User = user,
                Project = project,
                TaskItem = task,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };
            _unitOfWork.TimeEntries.Add(timeEntry);

            await _unitOfWork.SaveChangesAsync();
        }
    }
} 