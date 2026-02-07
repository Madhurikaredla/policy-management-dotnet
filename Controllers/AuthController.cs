using Microsoft.AspNetCore.Mvc;
using PolicyManagement.DTOs;
using PolicyManagement.Services;
using PolicyManagement.Constants;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly ILogger<AuthController> logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    [HttpPost("register", Name = "Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        // will be handled by dto validation attributes, but can also be checked here if needed
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        // }
        logger.LogInformation("User registration attempt for email: {Email}", registerDto.Email);


        try
        {
            var user = await authService.RegisterAsync(registerDto);
            logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);
            return Ok(new { message = ErrorMessages.Success, data = user });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Registration failed for {Email}: {Message}", registerDto.Email, ex.Message);
            return BadRequest(new { message = ErrorMessages.RegistrationFailed, error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during registration for {Email}", registerDto.Email);
            return StatusCode(500, new { message = ErrorMessages.InternalError, error = ex.Message });
        }
    }

    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // will be handled by dto validation attributes, but can also be checked here if needed
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        // }
        var identifier = loginDto.Type == LoginTypes.EMAIL ? loginDto.Email : $"{loginDto.CountryCode}{loginDto.PhoneNumber}";
        logger.LogInformation("Login attempt for {LoginType}: {Identifier}", loginDto.Type, identifier);

        try
        {
            var result = await authService.LoginAsync(loginDto);
            
            if (result == null)
            {
                logger.LogWarning("Failed login attempt for {LoginType}: {Identifier}", loginDto.Type, identifier);
                string errorMsg = loginDto.Type == LoginTypes.PHONE
                    ? ErrorMessages.InvalidPhoneOrPassword
                    : ErrorMessages.InvalidEmailOrPassword;
                return Unauthorized(new { message = errorMsg });
            }

            logger.LogInformation("Successful login for user ID: {UserId}", result.User.Id);
            return Ok(new { message = ErrorMessages.Success, data = result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during login for {LoginType}: {Identifier}", loginDto.Type, identifier);
            return StatusCode(500, new { message = ErrorMessages.InternalError, error = ex.Message });
        }
    }
}
