using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.App.Controllers;

// TODO Add Actionmethod for PUT method and respond with HTTP405 method not allowed.

/// <summary>
/// Endpoint of Userinteractions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserInteractionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get User interactions. By default, all Interactions. Allows filtering by <c>IsOpen</c> property in query string.
    /// </summary>
    /// <param name="isOpen">Ommiting this parameter will return all User interactions.</param>
    /// <returns>All or filtered by `IsOpen` collection of interactions.</returns>
    /// <param name="ct"></param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserInteractionDto>>> GetUserInteractions(bool? isOpen, CancellationToken ct)
    {
        UserInteractionGetQuery<UserInteractionDto> request = new(
            UserInteractionDto.Projection,
            model => !isOpen.HasValue || model.IsOpen == isOpen
            );

        (IEnumerable<UserInteractionDto> dtos, int totalCount) = await _mediator.Send(request, ct);

        return Ok(dtos);
    }

    /// <summary>
    /// Get Userinteraction by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionDto>> GetUserInteraction(Guid id, CancellationToken ct)
    {
        UserInteraction model = await _mediator.Send(new UserInteractionGetByIdQuery(id), ct);

        return Ok(UserInteractionDto.Projection.Compile().Invoke(model));
    }

    /// <summary>
    /// Patch UserInteraction model: change <c>IsOpen</c> state.
    /// </summary>
    /// <remarks>Patching is constrainted to set <c>IsOpen</c> property only,
    /// other props are ignored even if sent.
    /// </remarks>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchUserInteraction(Guid id, UserInteractionSetOpenStateCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        _ = await _mediator.Send(command, ct);

        return NoContent();
    }

    /// <summary>
    /// Create Userinteraction.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserInteractionDto>> PostUserInteraction(UserInteractionCreateCommand command, CancellationToken ct)
    {
        UserInteraction model = await _mediator.Send(command, ct);

        UserInteractionDto dto = UserInteractionDto.Projection.Compile().Invoke(model);

        return CreatedAtAction(nameof(GetUserInteraction), new { id = model.Id }, dto);
    }
}
