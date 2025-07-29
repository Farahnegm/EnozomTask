using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Application.Interfaces.Factories;
using EnozomTask.Application.Interfaces.Services;
using EnozomTask.Domain.Interfaces.Strategies;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;

namespace EnozomTask.InfraStructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEntityFactory _entityFactory;
        private readonly ISyncStrategy _syncStrategy;

        public ProjectService(
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

        public async Task<ProjectReadDto> CreateProjectAsync(ProjectCreateDto dto)
        {
            var project = _entityFactory.CreateProject(dto);
            _unitOfWork.Projects.Add(project);
            await _unitOfWork.SaveChangesAsync();
            
            var clockifyId = await _syncStrategy.SyncProjectAsync(project);
            if (!string.IsNullOrEmpty(clockifyId))
            {
                project.ClockifyId = clockifyId;
                _unitOfWork.Projects.Update(project);
                await _unitOfWork.SaveChangesAsync();
            }
            
            return _mapper.Map<ProjectReadDto>(project);
        }

        public async Task<IEnumerable<ProjectReadDto>> GetAllProjectsAsync()
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectReadDto>>(projects);
        }

        public async Task<ProjectReadDto> GetProjectByIdAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return null;
            return _mapper.Map<ProjectReadDto>(project);
        }

        public async Task<ProjectReadDto> UpdateProjectAsync(int id, ProjectUpdateDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return null;
            
            project.Name = dto.Name;
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<ProjectReadDto>(project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return;
            
            _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<object> AssignUsersToProjectAsync(int projectId, List<int> userIds)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            if (project == null) throw new ArgumentException("Project not found");
            
            if (string.IsNullOrEmpty(project.ClockifyId))
            {
                throw new InvalidOperationException("Project is not synced to external service. Please sync the project first.");
            }
            
            var assignedUsers = new List<User>();
            var missingExternalUsers = new List<int>();
            var invalidExternalIds = new List<int>();
            
            foreach (var userId in userIds)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) continue;
                
                if (string.IsNullOrEmpty(user.ClockifyId))
                {
                    missingExternalUsers.Add(user.UserId);
                    continue;
                }
                
                bool isValidExternalId = user.ClockifyId.Length == 24 && 
                                       System.Text.RegularExpressions.Regex.IsMatch(user.ClockifyId, "^[0-9a-fA-F]{24}$");
                
                if (!isValidExternalId)
                {
                    invalidExternalIds.Add(user.UserId);
                    continue;
                }
                
                assignedUsers.Add(user);
            }
            
            bool externalSyncSuccess = false;
            if (assignedUsers.Any())
            {
                var userExternalIds = assignedUsers.Select(u => u.ClockifyId).ToList();
                externalSyncSuccess = await _syncStrategy.AssignUsersToProjectAsync(project.ClockifyId, userExternalIds);
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            var assignees = assignedUsers.Select(u => new AssigneeDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                ClockifyId = u.ClockifyId
            }).ToList();
            
            var messages = new List<string>();
            if (missingExternalUsers.Any())
            {
                messages.Add($"Users {string.Join(", ", missingExternalUsers)} are not in {_syncStrategy.ProviderName}. Please invite them and update their external ID.");
            }
            if (invalidExternalIds.Any())
            {
                messages.Add($"Users {string.Join(", ", invalidExternalIds)} have invalid external ID format. Please update with valid 24-character hex IDs.");
            }
            if (externalSyncSuccess && assignedUsers.Any())
            {
                messages.Add($"Users validated in {_syncStrategy.ProviderName} workspace. Note: Project access must be managed through {_syncStrategy.ProviderName} web interface.");
            }
            
            return new { 
                projectId, 
                projectName = project.Name,
                assignees,
                missingExternalUsers, 
                invalidExternalIds,
                externalSyncSuccess,
                message = string.Join(" ", messages)
            };
        }
    }
} 