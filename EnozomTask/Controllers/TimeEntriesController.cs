using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EnozomTask.Application.DTOs;
using EnozomTask.InfraStructure.Services;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using AutoMapper;
using System.Collections.Generic;
using EnozomTask.Application.Interfaces.Services;

namespace EnozomTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeEntriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClockifySyncService _clockifySyncService;
        public TimeEntriesController(IUnitOfWork unitOfWork, IMapper mapper, IClockifySyncService clockifySyncService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clockifySyncService = clockifySyncService;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TimeEntrySimpleCreateDto dto)
        {
            var timeEntry = new TimeEntry
            {
                StartTime = dto.Start,
                EndTime = dto.End,
                TaskItemId = dto.TaskItemId,
                UserId = dto.UserId,
                ProjectId = dto.ProjectId
            };
            _unitOfWork.TimeEntries.Add(timeEntry);
            await _unitOfWork.SaveChangesAsync();
            timeEntry.Project = await _unitOfWork.Projects.GetByIdAsync(timeEntry.ProjectId);
            timeEntry.TaskItem = await _unitOfWork.TaskItems.GetByIdAsync(timeEntry.TaskItemId);
            var clockifyId = await _clockifySyncService.SyncTimeEntryAsync(timeEntry);
            if (!string.IsNullOrEmpty(clockifyId))
            {
                timeEntry.ClockifyId = clockifyId;
                _unitOfWork.TimeEntries.Update(timeEntry);
                await _unitOfWork.SaveChangesAsync();
            }
            var result = _mapper.Map<TimeEntryReadDto>(timeEntry);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entries = await _unitOfWork.TimeEntries.GetAllAsync();
            var result = _mapper.Map<IEnumerable<TimeEntryReadDto>>(entries);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
            if (entry == null) return NotFound();
            var result = _mapper.Map<TimeEntryReadDto>(entry);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TimeEntryUpdateDto dto)
        {
            var entry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
            if (entry == null) return NotFound();
            entry.UserId = dto.UserId;
            entry.ProjectId = dto.ProjectId;
            entry.TaskItemId = dto.TaskItemId;
            entry.StartTime = dto.Start;
            entry.EndTime = dto.End;
            _unitOfWork.TimeEntries.Update(entry);
            await _unitOfWork.SaveChangesAsync();
            return Ok(entry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
            if (entry == null) return NotFound();
            _unitOfWork.TimeEntries.Delete(entry);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 