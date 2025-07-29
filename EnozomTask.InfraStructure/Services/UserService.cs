using AutoMapper;
using EnozomTask.Application.DTOs;
using EnozomTask.Application.Interfaces.Services;
using EnozomTask.Domain.Entities;
using EnozomTask.Application.Interfaces.Factories;
using EnozomTask.Domain.Interfaces.Strategies;
using EnozomTask.Domain.Repositories;

namespace EnozomTask.InfraStructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEntityFactory _entityFactory;
        private readonly ISyncStrategy _syncStrategy;

        public UserService(
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

        public async Task<object> CreateUserAsync(UserCreateDto dto)
        {
            var user = _entityFactory.CreateUser(dto);
            _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<UserReadDto>(user);
            return new
            {
                user = result,
                instructions = $"Please invite this user to {_syncStrategy.ProviderName} workspace and update their external ID using the update endpoint."
            };
        }

        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return null;
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return null;
            user.FullName = dto.FullName;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return;
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserReadDto> UpdateUserExternalIdAsync(int userId, string externalId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return null;
            user.ClockifyId = externalId;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<object> GetExternalUsersAsync()
        {
            var externalUsers = await _syncStrategy.GetUsersAsync();
            return new
            {
                message = $"{_syncStrategy.ProviderName} users retrieved successfully",
                users = externalUsers
            };
        }
    }
} 