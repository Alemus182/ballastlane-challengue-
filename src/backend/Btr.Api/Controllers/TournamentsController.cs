using System.ComponentModel.DataAnnotations;
using Btr.Application.Features.Tournaments.Create;
using Btr.Application.Features.Tournaments.GetList;
using Microsoft.AspNetCore.Mvc;

namespace Btr.Api.Controllers;

[ApiController]
[Route("api/v1/tournaments")]
public class TournamentsController : ControllerBase
{
    private readonly CreateTournamentService _createTournamentService;
    private readonly GetTournamentsService _getTournamentsService;
    private readonly ILogger<TournamentsController> _logger;

    public TournamentsController(
        CreateTournamentService createTournamentService,
        GetTournamentsService getTournamentsService,
        ILogger<TournamentsController> logger)
    {
        _createTournamentService = createTournamentService;
        _getTournamentsService = getTournamentsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("List tournaments requested");
        var result = await _getTournamentsService.ExecuteAsync(cancellationToken);
        _logger.LogInformation("List tournaments returned {Count} items", result.Tournaments.Count);
        return Ok(result.Tournaments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTournamentApiRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Create tournament requested for {TournamentName} from {StartDate} to {EndDate}",
            request.Name,
            request.StartDate,
            request.EndDate);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create tournament rejected due to invalid model state");
            return ValidationProblem(ModelState);
        }

        var result = await _createTournamentService.ExecuteAsync(
            new CreateTournamentRequest(request.Name, request.Location, request.StartDate!.Value, request.EndDate!.Value),
            cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Create tournament failed for {TournamentName}: {Error}", request.Name, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Create tournament succeeded with id {TournamentId}", result.TournamentId);
        return Created($"/api/v1/tournaments/{result.TournamentId}", new { tournamentId = result.TournamentId });
    }

    public sealed class CreateTournamentApiRequest
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; init; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string Location { get; init; } = string.Empty;

        [Required]
        public DateTime? StartDate { get; init; }

        [Required]
        public DateTime? EndDate { get; init; }
    }
}
