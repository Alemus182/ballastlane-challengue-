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

    public TournamentsController(
        CreateTournamentService createTournamentService,
        GetTournamentsService getTournamentsService)
    {
        _createTournamentService = createTournamentService;
        _getTournamentsService = getTournamentsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getTournamentsService.ExecuteAsync(cancellationToken);
        return Ok(result.Tournaments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTournamentApiRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _createTournamentService.ExecuteAsync(
            new CreateTournamentRequest(request.Name, request.Location, request.StartDate!.Value, request.EndDate!.Value),
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

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
