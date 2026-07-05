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
    private readonly ILogger<RegistrationsController> _logger;

    public RegistrationsController(
        CreateRegistrationService createRegistrationService,
        GetRegistrationsService getRegistrationsService,
        GetRegistrationByIdService getRegistrationByIdService,
        UpdateRegistrationService updateRegistrationService,
        DeleteRegistrationService deleteRegistrationService,
        ILogger<RegistrationsController> logger)
    {
        _createRegistrationService = createRegistrationService;
        _getRegistrationsService = getRegistrationsService;
        _getRegistrationByIdService = getRegistrationByIdService;
        _updateRegistrationService = updateRegistrationService;
        _deleteRegistrationService = deleteRegistrationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("List registrations requested");
        var result = await _getRegistrationsService.ExecuteAsync(cancellationToken);
        _logger.LogInformation("List registrations returned {Count} items", result.Registrations.Count);
        return Ok(result.Registrations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get registration requested for id {RegistrationId}", id);

        if (!Guid.TryParse(id, out var parsedId))
        {
            _logger.LogWarning("Get registration rejected because id is invalid: {RegistrationId}", id);
            return BadRequest(new { error = "Invalid registration id." });
        }

        var result = await _getRegistrationByIdService.ExecuteAsync(parsedId, cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("Get registration not found for id {RegistrationId}", parsedId);
            return NotFound(new { error = result.Error });
        }

        _logger.LogInformation("Get registration succeeded for id {RegistrationId}", parsedId);
        return Ok(result.Registration);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationApiRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create registration requested for tournament {TournamentId}", request.TournamentId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create registration rejected due to invalid model state");
            return ValidationProblem(ModelState);
        }

        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            _logger.LogWarning("Create registration rejected due to invalid user context");
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
            _logger.LogWarning("Create registration failed for tournament {TournamentId}: {Error}", request.TournamentId, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Create registration succeeded with id {RegistrationId}", result.RegistrationId);
        return Created($"/api/v1/registrations/{result.RegistrationId}", new { registrationId = result.RegistrationId });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateRegistrationApiRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update registration requested for id {RegistrationId}", id);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Update registration rejected due to invalid model state for id {RegistrationId}", id);
            return ValidationProblem(ModelState);
        }

        if (!Guid.TryParse(id, out var parsedId))
        {
            _logger.LogWarning("Update registration rejected because id is invalid: {RegistrationId}", id);
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
                _logger.LogWarning("Update registration not found for id {RegistrationId}", parsedId);
                return NotFound(new { error = result.Error });
            }

            _logger.LogWarning("Update registration failed for id {RegistrationId}: {Error}", parsedId, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Update registration succeeded for id {RegistrationId}", parsedId);
        return Ok(new { registrationId = parsedId });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete registration requested for id {RegistrationId}", id);

        if (!Guid.TryParse(id, out var parsedId))
        {
            _logger.LogWarning("Delete registration rejected because id is invalid: {RegistrationId}", id);
            return BadRequest(new { error = "Invalid registration id." });
        }

        var result = await _deleteRegistrationService.ExecuteAsync(parsedId, cancellationToken);
        if (!result.Success)
        {
            if (result.Error == "Registration not found.")
            {
                _logger.LogWarning("Delete registration not found for id {RegistrationId}", parsedId);
                return NotFound(new { error = result.Error });
            }

            _logger.LogWarning("Delete registration failed for id {RegistrationId}: {Error}", parsedId, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Delete registration succeeded for id {RegistrationId}", parsedId);
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

