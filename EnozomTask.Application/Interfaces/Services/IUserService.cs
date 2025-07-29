using EnozomTask.Application.DTOs;

namespace EnozomTask.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<object> CreateUserAsync(UserCreateDto dto);
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto> GetUserByIdAsync(int id);
        Task<UserReadDto> UpdateUserAsync(int id, UserUpdateDto dto);
        Task DeleteUserAsync(int id);
        Task<UserReadDto> UpdateUserExternalIdAsync(int userId, string externalId);
        Task<object> GetExternalUsersAsync();
    }
} 