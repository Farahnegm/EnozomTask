using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.Services;
using EnozomTask.Application.Interfaces.Services;

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
            var result = _mapper.Map<UserReadDto>(user);
            return Ok(new { 
                user = result,
                instructions = "Please invite this user to Clockify workspace and update their ClockifyId using the PUT /api/Users/{userId}/clockify-id endpoint."
            });
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

        [HttpPut("{userId}/clockify-id")]
        public async Task<IActionResult> UpdateClockifyId(int userId, [FromBody] UpdateClockifyIdDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return NotFound();
            
            user.ClockifyId = dto.ClockifyId;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Ok(_mapper.Map<UserReadDto>(user));
        }

        [HttpGet("clockify")]
        public async Task<IActionResult> GetClockifyUsers()
        {
            try
            {
                var clockifyUsers = await _clockifySyncService.GetClockifyUsersAsync();
                return Ok(new { 
                    message = "Clockify users retrieved successfully",
                    users = clockifyUsers 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Failed to retrieve Clockify users", 
                    error = ex.Message 
                });
            }
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