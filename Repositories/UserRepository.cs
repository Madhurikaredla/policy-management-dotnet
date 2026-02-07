using PolicyManagement.Data;
using PolicyManagement.Models;
using Microsoft.EntityFrameworkCore;
using PolicyManagement.DTOs;

namespace PolicyManagement.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<UserRepository> logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        this.dbContext = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await this.dbContext.Users.ToListAsync();
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await this.dbContext.Users.FindAsync(id);
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await this.dbContext.Users
            .Where(u => EF.Functions.ILike(u.Email, email))
            .FirstOrDefaultAsync();
    }
    public async Task<User?> GetUserByPhoneNumberAsync(string countryCode, string phoneNumber)
    {
        return await this.dbContext.Users
            .Where(u => u.CountryCode == countryCode && u.PhoneNumber == phoneNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateUserAsync(CreateUserDto user)
    {
        logger.LogInformation("Creating user in database with email: {Email}", user.Email);
        var utcNow = DateTime.UtcNow;
        var newUser = new User
        {
            Name =  user.Name,
            Email = user.Email,
            PasswordHash = user.Password,
            Role = user.Role,
            CountryCode = user.CountryCode,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            Active = user.Active
        };
        this.dbContext.Users.Add(newUser);
        await this.dbContext.SaveChangesAsync();
        logger.LogInformation("User created in database with ID: {UserId}", newUser.Id);
        return newUser;
    }

    public async Task<User?> UpdateUserAsync(int id, UpdateUserDto user)
    {
        var existingUser = await this.dbContext.Users.FindAsync(id);
        if (existingUser == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(user.Name))
        {
            existingUser.Name = user.Name;
        }
        if (!string.IsNullOrEmpty(user.Email))
        {
            existingUser.Email = user.Email;
        }
        if (!string.IsNullOrEmpty(user.Password))
        {
            existingUser.PasswordHash = user.Password;
        }
        if (!string.IsNullOrEmpty(user.Role))
        {
            existingUser.Role = user.Role;
        }
        if (!string.IsNullOrEmpty(user.CountryCode))
        {
            existingUser.CountryCode = user.CountryCode;
        }
        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            existingUser.PhoneNumber = user.PhoneNumber;
        }
        if (user.Active.HasValue)
        {
            existingUser.Active = user.Active.Value;
        }
        var utcNow = DateTime.UtcNow;
        existingUser.UpdatedAt = utcNow;
        // Ensure CreatedAt is always UTC
        if (existingUser.CreatedAt.Kind != DateTimeKind.Utc)
        {
            existingUser.CreatedAt = DateTime.SpecifyKind(existingUser.CreatedAt, DateTimeKind.Utc);
        }

        await this.dbContext.SaveChangesAsync();
        return existingUser;
    }

    public async Task<User?> MarkUserAsInactiveAsync(int id)
    {
        var existingUser = await this.dbContext.Users.Where(u => u.Id == id && u.Active).FirstOrDefaultAsync();
        if (existingUser == null)
        {
            return null;
        }

        existingUser.Active = false;
        var utcNow = DateTime.UtcNow;
        existingUser.UpdatedAt = utcNow;
        // Ensure CreatedAt is always UTC
        if (existingUser.CreatedAt.Kind != DateTimeKind.Utc)
        {
            existingUser.CreatedAt = DateTime.SpecifyKind(existingUser.CreatedAt, DateTimeKind.Utc);
        }

        await this.dbContext.SaveChangesAsync();
        return existingUser;
    }

}