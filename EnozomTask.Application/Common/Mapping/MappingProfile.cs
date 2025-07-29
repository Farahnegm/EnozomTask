using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;

namespace EnozomTask
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectReadDto>();
            CreateMap<User, UserReadDto>();
            CreateMap<TaskItem, TaskItemReadDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : "N/A"))
                .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.FullName : "N/A"))
                .ForMember(dest => dest.AssignedUserClockifyId, opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.ClockifyId : null));
            CreateMap<TimeEntry, TimeEntryReadDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "N/A"))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : "N/A"))
                .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.TaskItem != null ? src.TaskItem.Name : "N/A"))
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.EndTime));
        }
    }
} 