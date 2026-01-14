using Application.DTOs.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> AssignRoleAsync(Guid userId, string role);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<UserInfoDto> GetUserInfoAsync(Guid userId);
}
