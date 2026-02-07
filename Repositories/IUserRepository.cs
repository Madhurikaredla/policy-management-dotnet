namespace PolicyManagement.Repositories;

using PolicyManagement.DTOs;
using PolicyManagement.Models;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User?> GetUserByIdAsync(int id);

    Task<User?> GetUserByEmailAsync(string email);

    Task<User?> GetUserByPhoneNumberAsync(string countryCode, string phoneNumber);

    Task<User> CreateUserAsync(CreateUserDto user);

    Task<User?> UpdateUserAsync(int id, UpdateUserDto user);

    Task<User?> MarkUserAsInactiveAsync(int id);
}