using PolicyManagement.DTOs;
using PolicyManagement.Models;

namespace PolicyManagement.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);

    Task<UserResponseDto?> GetUserByEmailAsync(string email);

    Task<UserResponseDto?> GetUserByPhoneNumberAsync(string countryCode, string phoneNumber);

    Task<UserResponseDto> CreateUserAsync(CreateUserDto user);

    Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto user);

    Task<UserResponseDto?> MarkUserAsInactiveAsync(int id);
    
}