using EnozomTask.Application.DTOs;
using EnozomTask.Application.Interfaces.Factories;
using EnozomTask.Domain.Entities;

namespace EnozomTask.InfraStructure.Factories
{
    public class EntityFactory : IEntityFactory
    {
        public Project CreateProject(ProjectCreateDto dto)
        {
            return new Project
            {
                Name = dto.Name
            };
        }

        public TaskItem CreateTaskItem(TaskItemCreateDto dto, Project project, User assignedUser)
        {
            return new TaskItem
            {
                Name = dto.Title,
                EstimateHours = dto.EstimateHours,
                ProjectId = dto.ProjectId,
                UserId = dto.AssignedUserId,
                Project = project,
                AssignedUser = assignedUser
            };
        }

        public User CreateUser(UserCreateDto dto)
        {
            return new User
            {
                FullName = dto.FullName
            };
        }

        public TimeEntry CreateTimeEntry(TimeEntrySimpleCreateDto dto, Project project, TaskItem taskItem, User user)
        {
            return new TimeEntry
            {
                StartTime = dto.Start,
                EndTime = dto.End,
                ProjectId = dto.ProjectId,
                TaskItemId = dto.TaskItemId,
                UserId = dto.UserId,
                Project = project,
                TaskItem = taskItem,
                User = user
            };
        }
    }
} 