using PolicyManagement.Constants;
using PolicyManagement.DTOs;
using PolicyManagement.Models;
using PolicyManagement.Repositories;

namespace PolicyManagement.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IConfiguration configuration;
    private readonly ILogger<UserService> logger;

    public UserService(IUserRepository userRepositoryObject, IConfiguration configuration, ILogger<UserService> logger)
    {
        this.userRepository = userRepositoryObject;
        this.configuration = configuration;
        this.logger = logger;
    }
    
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await this.userRepository.GetAllUsersAsync();
        return users.Select(u => ToUserDto(u));
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await this.userRepository.GetUserByIdAsync(id);
        return user == null ? null : ToUserDto(user);
    }

    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await this.userRepository.GetUserByEmailAsync(email);
        return user == null ? null : ToUserDto(user);
    }

    public async Task<UserResponseDto?> GetUserByPhoneNumberAsync(string countryCode, string phoneNumber)
    {
        var user = await this.userRepository.GetUserByPhoneNumberAsync(countryCode, phoneNumber);
        return user == null ? null : ToUserDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto user)
    {
        logger.LogInformation("Creating new user with email: {Email}", user.Email);

        // Check if user with email already exists
        var existingUserByEmail = await this.userRepository.GetUserByEmailAsync(user.Email);
        if (existingUserByEmail != null)
        {
            logger.LogWarning(ErrorMessages.UserAlreadyExists);
            throw new InvalidOperationException(ErrorMessages.UserAlreadyExists);
        }

        // Check if user with phone number already exists (if phone is provided)
        if (!string.IsNullOrEmpty(user.CountryCode) && !string.IsNullOrEmpty(user.PhoneNumber))
        {
            var existingUserByPhone = await this.userRepository.GetUserByPhoneNumberAsync(user.CountryCode, user.PhoneNumber);
            if (existingUserByPhone != null)
            {
                logger.LogWarning(ErrorMessages.UserAlreadyExists);
                throw new InvalidOperationException(ErrorMessages.UserAlreadyExists);
            }
        }

        // update the password hash before creating the user
        user.Password = HashPassword(user.Password);
        // Ensure role is ROLE_USER or ROLE_ADMIN
        if (user.Role != Roles.ROLE_USER && user.Role != Roles.ROLE_ADMIN)
        {
            user.Role = Roles.ROLE_USER;
        }
        var createdUser = await this.userRepository.CreateUserAsync(user);
        logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
        return ToUserDto(createdUser);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto user)
    {
        logger.LogInformation("Updating user with ID: {UserId}", id);
        var existingUser = await this.userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            logger.LogWarning("User with ID {UserId} not found for update", id);
            return null;
        }

        // Check if email is being updated and if new email already exists
        if (!string.IsNullOrEmpty(user.Email) && user.Email != existingUser.Email)
        {
            var userWithEmail = await this.userRepository.GetUserByEmailAsync(user.Email);
            if (userWithEmail != null)
            {
                logger.LogWarning(ErrorMessages.UserAlreadyExists);
                throw new InvalidOperationException(ErrorMessages.UserAlreadyExists);
            }
        }

        // Check if phone is being updated and if new phone already exists
        if (!string.IsNullOrEmpty(user.CountryCode) && !string.IsNullOrEmpty(user.PhoneNumber))
        {
            if (user.CountryCode != existingUser.CountryCode || user.PhoneNumber != existingUser.PhoneNumber)
            {
                var userWithPhone = await this.userRepository.GetUserByPhoneNumberAsync(user.CountryCode, user.PhoneNumber);
                if (userWithPhone != null && userWithPhone.Id != id)
                {
                    logger.LogWarning(ErrorMessages.UserAlreadyExists);
                    throw new InvalidOperationException(ErrorMessages.UserAlreadyExists);
                }
            }
        }

        // if password is being updated, hash the new password
        if (!string.IsNullOrEmpty(user.Password))
        {
            user.Password = HashPassword(user.Password);
        }

        var updatedUser = await this.userRepository.UpdateUserAsync(id, user);
        logger.LogInformation("User with ID {UserId} updated successfully", id);
        return updatedUser == null ? null : ToUserDto(updatedUser);
    }

    public async Task<UserResponseDto?> MarkUserAsInactiveAsync(int id)
    {
        var user = await this.userRepository.MarkUserAsInactiveAsync(id);
        return user == null ? null : ToUserDto(user);
    }

    // Method to hash password using HMACSHA256 and fetch secret key from appsettings.json
    public string HashPassword(string password)
    {
        var secretKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Secret key not found in appsettings.json under Jwt:Key");
        }
        using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);
    }

    // Helper method to map User to UserResponseDto (excludes password hash)
    private UserResponseDto ToUserDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            CountryCode = user.CountryCode ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow,
            Active = user.Active
        };
    }
    
}