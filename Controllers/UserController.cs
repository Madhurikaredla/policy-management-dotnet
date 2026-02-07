using Microsoft.AspNetCore.Mvc;
using PolicyManagement.DTOs;
using PolicyManagement.Services;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<UserController> logger;

    public UserController(IUserService userServiceObject, ILogger<UserController> logger)
    {
        this.userService = userServiceObject;
        this.logger = logger;
    }
    
    [HttpGet (Name = "GetUsers")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            logger.LogInformation("Fetching all users");
            var users = await this.userService.GetAllUsersAsync();
            logger.LogInformation("Retrieved {Count} users", users.Count());
            return Ok(new { message = "Users retrieved successfully", data = users });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching all users");
            return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
        }
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            logger.LogInformation("Fetching user with ID: {UserId}", id);
            var user = await this.userService.GetUserByIdAsync(id);
            if (user is null)
            {
                logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }
            return Ok(new { message = "User retrieved successfully", data = user });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching user with ID: {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
        }
    }

    [HttpGet("by-email", Name = "GetUserByEmail")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        try
        {
            logger.LogInformation("Fetching user by email: {Email}", email);
            var user = await this.userService.GetUserByEmailAsync(email);
            if (user is null)
            {
                logger.LogWarning("User with email {Email} not found", email);
                return NotFound(new { message = $"User with email {email} not found" });
            }
            logger.LogInformation("User with email {Email} retrieved successfully", email);
            return Ok(new { message = "User retrieved successfully", data = user });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching user by email: {Email}", email);
            return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
        }
    }

    // for get from body use [FromBody]
    [HttpGet("by-phone", Name = "GetUserByPhoneNumber")]
    public async Task<IActionResult> GetUserByPhoneNumber([FromQuery] string countryCode, [FromQuery] string phoneNumber)
    {
        try
        {
            logger.LogInformation("Fetching user by phone: {CountryCode}{PhoneNumber}", countryCode, phoneNumber);
            var user = await this.userService.GetUserByPhoneNumberAsync(countryCode, phoneNumber);
            if (user is null)
            {
                logger.LogWarning("User with phone number {CountryCode}{PhoneNumber} not found", countryCode, phoneNumber);
                return NotFound(new { message = $"User with phone number {countryCode} {phoneNumber} not found" });
            }
            logger.LogInformation("User with phone number {CountryCode}{PhoneNumber} retrieved successfully", countryCode, phoneNumber);
            return Ok(new { message = "User retrieved successfully", data = user });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching user by phone: {CountryCode}{PhoneNumber}", countryCode, phoneNumber);
            return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
        }
    }

    [HttpPost (Name = "CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
    {
        logger.LogInformation("Creating new user with email: {Email}", user.Email);
        try
        {
            var createdUser = await this.userService.CreateUserAsync(user);
            logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
            return CreatedAtRoute("GetUserById", new { id = createdUser.Id }, new { message = "User created successfully", data = createdUser });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("User creation failed: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}", Name = "UpdateUser")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto user)
    {
        logger.LogInformation("Attempting to update user with ID: {UserId}", id);
        try
        {
            var updatedUser = await this.userService.UpdateUserAsync(id, user);
            if (updatedUser is null)
            {
                logger.LogWarning("User with ID {UserId} not found for update", id);
                return NotFound(new { message = $"User with ID {id} not found" });  
            }
            logger.LogInformation("User with ID {UserId} updated successfully", id);
            return Ok(new { message = "User updated successfully", data = updatedUser });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("User update failed: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}", Name = "MarkUserAsInactive")]
    public async Task<IActionResult> MarkUserAsInactive(int id)
    {
        try
        {
            logger.LogInformation("Attempting to mark user as inactive with ID: {UserId}", id);
            var inactiveUser = await this.userService.MarkUserAsInactiveAsync(id);
            if (inactiveUser is null)
            {
                logger.LogWarning("User with ID {UserId} not found for inactivation", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }
            logger.LogInformation("User with ID {UserId} marked as inactive successfully", id);
            return Ok(new { message = "User marked as inactive successfully", data = inactiveUser });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking user as inactive with ID: {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while marking user as inactive", error = ex.Message });
        }
    }


}
