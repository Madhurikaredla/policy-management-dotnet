using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PolicyManagement.Constants;
using PolicyManagement.DTOs;
using PolicyManagement.Repositories;

namespace PolicyManagement.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IConfiguration configuration;
    private readonly ILogger<AuthService> logger;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        this.userRepository = userRepository;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        logger.LogInformation("Processing login request for type: {LoginType}", loginDto.Type);
        Models.User? user = null;

        // Find user by email or phone based on login type
        if (loginDto.Type == LoginTypes.EMAIL)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                logger.LogWarning(ErrorMessages.InvalidEmailOrPassword);
                return null;
            }
            user = await userRepository.GetUserByEmailAsync(loginDto.Email);
        }
        else if (loginDto.Type == LoginTypes.PHONE)
        {
            if (string.IsNullOrWhiteSpace(loginDto.CountryCode) || string.IsNullOrWhiteSpace(loginDto.PhoneNumber))
            {
                logger.LogWarning(ErrorMessages.InvalidPhoneOrPassword);
                return null;
            }
            user = await userRepository.GetUserByPhoneNumberAsync(loginDto.CountryCode, loginDto.PhoneNumber);
        }
        else
        {
            logger.LogWarning(ErrorMessages.InvalidEmailOrPassword);
            return null;
        }

        if (user == null)
        {
            logger.LogWarning(ErrorMessages.UserNotFound);
            return null;
        }

        // Verify password
        var hashedPassword = HashPassword(loginDto.Password);
        if (user.PasswordHash != hashedPassword)
        {
            logger.LogWarning(ErrorMessages.InvalidEmailOrPassword);
            return null;
        }

        // Check if user is active
        if (!user.Active)
        {
            logger.LogWarning(ErrorMessages.UserInactive);
            return null;
        }

        logger.LogInformation("Login successful for user ID: {UserId}", user.Id);
        // Generate JWT token
        var token = GenerateJwtToken(user);

        // Map user to UserDto
        var userDto = new UserResponseDto
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

        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        logger.LogInformation("Processing registration for email: {Email}", registerDto.Email);

        // Check if user already exists with the same email
        var existingUser = await userRepository.GetUserByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            logger.LogWarning(ErrorMessages.EmailAlreadyExists);
            throw new InvalidOperationException(ErrorMessages.EmailAlreadyExists);
        }

        // Check if user already exists with the same phone number (if provided)
        if (!string.IsNullOrWhiteSpace(registerDto.CountryCode) && !string.IsNullOrWhiteSpace(registerDto.PhoneNumber))
        {
            existingUser = await userRepository.GetUserByPhoneNumberAsync(registerDto.CountryCode, registerDto.PhoneNumber);
            if (existingUser != null)            {
                logger.LogWarning(ErrorMessages.PhoneAlreadyExists);
                throw new InvalidOperationException(ErrorMessages.PhoneAlreadyExists);
            }
        }

        // Create user DTO
        var createUserDto = new CreateUserDto
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = registerDto.Password,
            Role = registerDto.Role,
            CountryCode = registerDto.CountryCode,
            PhoneNumber = registerDto.PhoneNumber,
            Active = registerDto.Active
        };

        // Hash password
        createUserDto.Password = HashPassword(createUserDto.Password);

        // Create user
        var user = await userRepository.CreateUserAsync(createUserDto);

        // Map to UserDto
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

    private string GenerateJwtToken(Models.User user)
    {
        var jwtKey = configuration["Jwt:Key"];
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];
        var jwtExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"] ?? "60");

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key not found in configuration");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExpireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        var secretKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Secret key not found in appsettings.json under Jwt:Key");
        }
        using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);
    }
}
