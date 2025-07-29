using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.Application.Services;

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