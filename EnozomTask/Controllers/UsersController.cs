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
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClockifySyncService _clockifySyncService;
        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IClockifySyncService clockifySyncService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clockifySyncService = clockifySyncService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserCreateDto dto)
        {
            var user = new User { FullName = dto.FullName };
            _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();
            // Removed call to _clockifySyncService.SyncUserAsync(user) as user creation is not supported
            var result = _mapper.Map<UserReadDto>(user);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var result = _mapper.Map<IEnumerable<UserReadDto>>(users);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return NotFound();
            var result = _mapper.Map<UserReadDto>(user);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.FullName = dto.FullName;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return NotFound();
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 