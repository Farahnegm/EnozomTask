using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Application.Interfaces.Services;
using EnozomTask.Domain.Entities;
using EnozomTask.Application.Interfaces.Factories;
using EnozomTask.Domain.Interfaces.Strategies;
using EnozomTask.Domain.Repositories;

namespace EnozomTask.InfraStructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEntityFactory _entityFactory;
        private readonly ISyncStrategy _syncStrategy;

        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEntityFactory entityFactory,
            ISyncStrategy syncStrategy)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _entityFactory = entityFactory;
            _syncStrategy = syncStrategy;
        }

        public async Task<object> CreateTaskAsync(TaskItemCreateDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId);
            if (project == null) throw new ArgumentException("Project not found");

            var assignedUser = await _unitOfWork.Users.GetByIdAsync(dto.AssignedUserId);
            if (assignedUser == null) throw new ArgumentException("Assigned user not found");

            var task = _entityFactory.CreateTaskItem(dto, project, assignedUser);
            _unitOfWork.TaskItems.Add(task);
            await _unitOfWork.SaveChangesAsync();

            task.Project = project;
            task.AssignedUser = assignedUser;

            bool hasValidExternalId = !string.IsNullOrEmpty(task.AssignedUser?.ClockifyId) &&
                                    task.AssignedUser.ClockifyId.Length == 24 &&
                                    System.Text.RegularExpressions.Regex.IsMatch(task.AssignedUser.ClockifyId, "^[0-9a-fA-F]{24}$");

            if (!hasValidExternalId)
            {
                var result = _mapper.Map<TaskItemReadDto>(task);
                return new
                {
                    task = result,
                    assignee = task.AssignedUser != null ? new AssigneeDto
                    {
                        UserId = task.AssignedUser.UserId,
                        FullName = task.AssignedUser.FullName,
                        ClockifyId = task.AssignedUser.ClockifyId
                    } : null,
                    message = "Task saved locally. User has invalid or missing external ID - task not synced to external service."
                };
            }

            var externalId = await _syncStrategy.SyncTaskItemAsync(task);
            if (!string.IsNullOrEmpty(externalId))
            {
                task.ClockifyId = externalId;
                _unitOfWork.TaskItems.Update(task);
                await _unitOfWork.SaveChangesAsync();
            }

            var finalResult = _mapper.Map<TaskItemReadDto>(task);
            return new
            {
                task = finalResult,
                assignee = new AssigneeDto
                {
                    UserId = task.AssignedUser.UserId,
                    FullName = task.AssignedUser.FullName,
                    ClockifyId = task.AssignedUser.ClockifyId
                },
                message = "Task saved locally and synced to external service."
            };
        }

        public async Task<IEnumerable<TaskItemReadDto>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskItemReadDto>>(tasks);
        }

        public async Task<TaskItemReadDto> GetTaskByIdAsync(int id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return null;
            return _mapper.Map<TaskItemReadDto>(task);
        }

        public async Task<TaskItemReadDto> UpdateTaskAsync(int id, TaskItemUpdateDto dto)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return null;

            task.Name = dto.Title;
            task.EstimateHours = dto.EstimateHours;
            task.ProjectId = dto.ProjectId;
            task.UserId = dto.AssignedUserId;

            _unitOfWork.TaskItems.Update(task);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskItemReadDto>(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return;

            _unitOfWork.TaskItems.Delete(task);
            await _unitOfWork.SaveChangesAsync();
        }
    }
} 