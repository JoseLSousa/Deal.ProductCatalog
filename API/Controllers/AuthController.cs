using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        try
        {
            await authService.RegisterAsync(registerDto);
            return Ok(new { message = "Usuário registrado com sucesso." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao registrar usuário.", detail = ex.Message });
        }
    }
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao fazer login.", detail = ex.Message });
        }
    }

    [HttpPost("assign-role")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            var success = await authService.AssignRoleAsync(assignRoleDto.UserId, assignRoleDto.Role);
            if (success)
            {
                return Ok(new { message = $"Role '{assignRoleDto.Role}' atribuída com sucesso." });
            }
            return BadRequest(new { message = "Falha ao atribuir role." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atribuir role.", detail = ex.Message });
        }
    }
    [HttpGet("my-roles")]
    [Authorize]
    public async Task<IActionResult> GetMyRoles()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var roles = await authService.GetUserRolesAsync(userId);
            return Ok(new { roles });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao obter roles.", detail = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var userInfo = await authService.GetUserInfoAsync(userId);
            return Ok(userInfo);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao obter informações do usuário.", detail = ex.Message });
        }
    }
}
