namespace Application.DTOs.Auth;

public record RegisterUserDto(
    string Email,
    string Password,
    string Role
);