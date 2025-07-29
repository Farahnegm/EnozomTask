using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using AutoMapper;
using System.Collections.Generic;
using EnozomTask.Application.Services;

namespace EnozomTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClockifySyncService _clockifySyncService;
        public TasksController(IUnitOfWork unitOfWork, IMapper mapper, IClockifySyncService clockifySyncService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clockifySyncService = clockifySyncService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskItemCreateDto dto)
        {
            var task = new TaskItem
            {
                Name = dto.Title,
                EstimateHours = dto.EstimateHours,
                ProjectId = dto.ProjectId,
                UserId = dto.AssignedUserId
            };
            _unitOfWork.TaskItems.Add(task);
            await _unitOfWork.SaveChangesAsync();
            // Load related project and assigned user for Clockify sync
            task.Project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            task.AssignedUser = await _unitOfWork.Users.GetByIdAsync(task.UserId);
            // Check if assigned user has a valid ClockifyId
            bool hasValidClockifyId = !string.IsNullOrEmpty(task.AssignedUser?.ClockifyId) && 
                                     task.AssignedUser.ClockifyId.Length == 24 && 
                                     System.Text.RegularExpressions.Regex.IsMatch(task.AssignedUser.ClockifyId, "^[0-9a-fA-F]{24}$");
            
            if (!hasValidClockifyId)
            {
                var result = _mapper.Map<TaskItemReadDto>(task);
                return Ok(new { 
                    task = result, 
                    assignee = task.AssignedUser != null ? new AssigneeDto
                    {
                        UserId = task.AssignedUser.UserId,
                        FullName = task.AssignedUser.FullName,
                        ClockifyId = task.AssignedUser.ClockifyId
                    } : null,
                    message = "Task saved locally. User has invalid or missing ClockifyId - task not synced to Clockify." 
                });
            }
            var clockifyId = await _clockifySyncService.SyncTaskItemAsync(task);
            if (!string.IsNullOrEmpty(clockifyId))
            {
                task.ClockifyId = clockifyId;
                _unitOfWork.TaskItems.Update(task);
                await _unitOfWork.SaveChangesAsync();
            }
            var finalResult = _mapper.Map<TaskItemReadDto>(task);
            return Ok(new { 
                task = finalResult, 
                assignee = new AssigneeDto
                {
                    UserId = task.AssignedUser.UserId,
                    FullName = task.AssignedUser.FullName,
                    ClockifyId = task.AssignedUser.ClockifyId
                },
                message = "Task saved locally and synced to Clockify." 
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            var result = _mapper.Map<IEnumerable<TaskItemReadDto>>(tasks);
            return Ok(new { tasks = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return NotFound();
            var result = _mapper.Map<TaskItemReadDto>(task);
            var assignee = task.AssignedUser != null ? new AssigneeDto
            {
                UserId = task.AssignedUser.UserId,
                FullName = task.AssignedUser.FullName,
                ClockifyId = task.AssignedUser.ClockifyId
            } : null;
            return Ok(new { task = result, assignee });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItemUpdateDto dto)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return NotFound();
            task.Name = dto.Title;
            task.EstimateHours = dto.EstimateHours;
            task.ProjectId = dto.ProjectId;
            task.UserId = dto.AssignedUserId;
            _unitOfWork.TaskItems.Update(task);
            await _unitOfWork.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return NotFound();
            _unitOfWork.TaskItems.Delete(task);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 