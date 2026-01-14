namespace Application.DTOs.Auth;

public record RegisterUserDto(
    string Username,
    string Email,
    string Password,
    string Role
);