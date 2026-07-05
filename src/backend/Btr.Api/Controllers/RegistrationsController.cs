using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Btr.Application.Features.Registrations.Create;
using Btr.Application.Features.Registrations.Delete;
using Btr.Application.Features.Registrations.Get;
using Btr.Application.Features.Registrations.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Btr.Api.Controllers;

[ApiController]
[Route("api/v1/registrations")]
public class RegistrationsController : ControllerBase
{
    private readonly CreateRegistrationService _createRegistrationService;
    private readonly GetRegistrationsService _getRegistrationsService;
    private readonly GetRegistrationByIdService _getRegistrationByIdService;
    private readonly UpdateRegistrationService _updateRegistrationService;
    private readonly DeleteRegistrationService _deleteRegistrationService;

    public RegistrationsController(
        CreateRegistrationService createRegistrationService,
        GetRegistrationsService getRegistrationsService,
        GetRegistrationByIdService getRegistrationByIdService,
        UpdateRegistrationService updateRegistrationService,
        DeleteRegistrationService deleteRegistrationService)
    {
        _createRegistrationService = createRegistrationService;
        _getRegistrationsService = getRegistrationsService;
        _getRegistrationByIdService = getRegistrationByIdService;
        _updateRegistrationService = updateRegistrationService;
        _deleteRegistrationService = deleteRegistrationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getRegistrationsService.ExecuteAsync(cancellationToken);
        return Ok(result.Registrations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var parsedId))
        {
            return BadRequest(new { error = "Invalid registration id." });
        }

        var result = await _getRegistrationByIdService.ExecuteAsync(parsedId, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Registration);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationApiRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized(new { error = "Invalid user context." });
        }

        var result = await _createRegistrationService.ExecuteAsync(
            new CreateRegistrationRequest(
                request.TournamentId,
                request.PlayerName,
                request.Nickname,
                request.ContactInfo),
            parsedUserId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Created($"/api/v1/registrations/{result.RegistrationId}", new { registrationId = result.RegistrationId });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateRegistrationApiRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!Guid.TryParse(id, out var parsedId))
        {
            return BadRequest(new { error = "Invalid registration id." });
        }

        var result = await _updateRegistrationService.ExecuteAsync(
            parsedId,
            new UpdateRegistrationRequest(
                request.TournamentId,
                request.PlayerName,
                request.Nickname,
                request.ContactInfo),
            cancellationToken);

        if (!result.Success)
        {
            if (result.Error == "Registration not found.")
            {
                return NotFound(new { error = result.Error });
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(new { registrationId = parsedId });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var parsedId))
        {
            return BadRequest(new { error = "Invalid registration id." });
        }

        var result = await _deleteRegistrationService.ExecuteAsync(parsedId, cancellationToken);
        if (!result.Success)
        {
            if (result.Error == "Registration not found.")
            {
                return NotFound(new { error = result.Error });
            }

            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    public sealed class CreateRegistrationApiRequest
    {
        [Required]
        public Guid TournamentId { get; init; }

        [Required]
        [MaxLength(256)]
        public string PlayerName { get; init; } = string.Empty;

        [MaxLength(128)]
        public string? Nickname { get; init; }

        [Required]
        [MaxLength(512)]
        public string ContactInfo { get; init; } = string.Empty;
    }

    public sealed class UpdateRegistrationApiRequest
    {
        [Required]
        public Guid TournamentId { get; init; }

        [Required]
        [MaxLength(256)]
        public string PlayerName { get; init; } = string.Empty;

        [MaxLength(128)]
        public string? Nickname { get; init; }

        [Required]
        [MaxLength(512)]
        public string ContactInfo { get; init; } = string.Empty;
    }
}

