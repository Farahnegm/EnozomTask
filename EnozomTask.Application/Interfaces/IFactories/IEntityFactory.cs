using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;

namespace EnozomTask.Application.Interfaces.Factories
{
    public interface IEntityFactory
    {
        Project CreateProject(ProjectCreateDto dto);
        TaskItem CreateTaskItem(TaskItemCreateDto dto, Project project, User assignedUser);
        User CreateUser(UserCreateDto dto);
        TimeEntry CreateTimeEntry(TimeEntrySimpleCreateDto dto, Project project, TaskItem taskItem, User user);
    }
} 