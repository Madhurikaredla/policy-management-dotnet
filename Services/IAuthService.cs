using PolicyManagement.DTOs;

namespace PolicyManagement.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserResponseDto> RegisterAsync(RegisterDto registerDto);
}
