using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Constants;
using Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infra.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IConfiguration configuration) : IAuthService
{
    public async Task RegisterAsync(RegisterUserDto registerDto)
    {
        // Validar role
        if (!IsValidRole(registerDto.Role))
        {
            throw new ArgumentException($"Role inválida. Use: {Roles.Admin}, {Roles.Editor} ou {Roles.Viewer}");
        }

        // Verificar se o usuário já existe
        var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Nome de usuário já existe.");
        }

        var existingEmail = await userManager.FindByEmailAsync(registerDto.Email);
        if (existingEmail != null)
        {
            throw new InvalidOperationException("Email já está em uso.");
        }

        // Criar usuário
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            EmailConfirmed = true // Para simplificar, confirmando automaticamente
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Erro ao criar usuário: {errors}");
        }

        // Atribuir role
        await EnsureRoleExistsAsync(registerDto.Role);
        await userManager.AddToRoleAsync(user, registerDto.Role);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email) ??
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        var roles = await userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? Roles.Viewer;

        var (Token, Expiration) = await GenerateJwtToken(user);

        return new AuthResponseDto(
            Token: Token,
            Email: user.Email!,
            Role: primaryRole,
            Expiration: Expiration
        );
    }

    public async Task<bool> AssignRoleAsync(Guid userId, string role)
    {
        if (!IsValidRole(role))
        {
            throw new ArgumentException($"Role inválida. Use: {Roles.Admin}, {Roles.Editor} ou {Roles.Viewer}");
        }

        var user = await userManager.FindByIdAsync(userId.ToString()) ??
            throw new KeyNotFoundException("Usuário não encontrado.");

        await EnsureRoleExistsAsync(role);

        // Remover roles antigas
        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);

        // Adicionar nova role
        var result = await userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ??
            throw new KeyNotFoundException("Usuário não encontrado.");
        return await userManager.GetRolesAsync(user);
    }

    public async Task<UserInfoDto> GetUserInfoAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ??
            throw new KeyNotFoundException("Usuário não encontrado.");
        
        var roles = await userManager.GetRolesAsync(user);

        return new UserInfoDto
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Roles = roles.ToList()
        };
    }

    private async Task<(string Token, DateTime Expiration)> GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey não configurada");

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Adicionar roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(
            int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }

    private async Task EnsureRoleExistsAsync(string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }
    }

    private static bool IsValidRole(string role)
    {
        return role == Roles.Admin || role == Roles.Editor || role == Roles.Viewer;
    }
}
