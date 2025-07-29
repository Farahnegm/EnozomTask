using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.Application.Services;
using System.Collections.Generic;
using System.Linq;

namespace EnozomTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClockifySyncService _clockifySyncService;
        public ProjectsController(IUnitOfWork unitOfWork, IMapper mapper, IClockifySyncService clockifySyncService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clockifySyncService = clockifySyncService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectCreateDto dto)
        {
            var project = new Project { Name = dto.Name };
            _unitOfWork.Projects.Add(project);
            await _unitOfWork.SaveChangesAsync();
            // Sync to Clockify
            var clockifyId = await _clockifySyncService.SyncProjectAsync(project);
            if (!string.IsNullOrEmpty(clockifyId))
            {
                project.ClockifyId = clockifyId;
                _unitOfWork.Projects.Update(project);
                await _unitOfWork.SaveChangesAsync();
            }
            var result = _mapper.Map<ProjectReadDto>(project);
            return Ok(result);
        }

        [HttpPost("{projectId}/assign-users")]
        public async Task<IActionResult> AssignUsers(int projectId, [FromBody] List<int> userIds)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            if (project == null) return NotFound();
            var assignedUsers = new List<User>();
            var missingClockifyUsers = new List<int>();
            foreach (var userId in userIds)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) continue;
                if (string.IsNullOrEmpty(user.ClockifyId))
                {
                    missingClockifyUsers.Add(user.UserId);
                    continue;
                }
               
                assignedUsers.Add(user);
            }
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { projectId, assignedUsers = assignedUsers.Select(u => u.UserId), missingClockifyUsers, message = missingClockifyUsers.Any() ? "Some users are not in Clockify. Please invite them and update their ClockifyId." : null });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ProjectReadDto>>(projects);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return NotFound();
            var result = _mapper.Map<ProjectReadDto>(project);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return NotFound();
            project.Name = dto.Name;
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();
            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return NotFound();
            _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 