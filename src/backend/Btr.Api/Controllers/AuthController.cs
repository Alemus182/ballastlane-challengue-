using System.ComponentModel.DataAnnotations;
using Btr.Application.Features.Auth.Login;
using Btr.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace Btr.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserService _registerUserService;
    private readonly LoginUserService _loginUserService;

    public AuthController(RegisterUserService registerUserService, LoginUserService loginUserService)
    {
        _registerUserService = registerUserService;
        _loginUserService = loginUserService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserApiRequest request, CancellationToken cancellationToken)
    {
        var result = await _registerUserService.ExecuteAsync(
            new RegisterUserRequest(request.UsernameOrEmail, request.Password),
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Created("/api/v1/auth/register", new { userId = result.UserId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserApiRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginUserService.ExecuteAsync(
            new LoginUserRequest(request.UsernameOrEmail, request.Password),
            cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(new { error = result.Error });
        }

        return Ok(new { accessToken = result.AccessToken });
    }

    public sealed class RegisterUserApiRequest
    {
        [Required]
        [MaxLength(256)]
        public string UsernameOrEmail { get; init; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; init; } = string.Empty;
    }

    public sealed class LoginUserApiRequest
    {
        [Required]
        [MaxLength(256)]
        public string UsernameOrEmail { get; init; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; init; } = string.Empty;
    }
}
