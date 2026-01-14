namespace Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    string Username,
    string Email,
    string Role,
    DateTime Expiration
);