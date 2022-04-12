using Backend.WebApi.App.ActionResults;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Extensions;
using Backend.WebApi.App.Filters;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Backend.WebApi.App.Controllers;

/// <summary>
/// Endpoint of Userinteractions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(OperationCancelledExceptionFilter))]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(499)]
public class UserInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserInteractionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Get User interactions. By default, all Interactions. Allows filtering by <c>IsOpen</c> property in query string.
    /// </summary>
    /// <param name="isOpen">Ommiting this parameter will return all User interactions.</param>
    /// <param name="ct"></param>
    /// <returns>All or filtered by `IsOpen` collection of interactions.</returns>
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
    [HttpGet("{Id}")]
    [ServiceFilter(typeof(IfNoneMatchFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionDto>> GetUserInteraction([FromRoute] UserInteractionGetByIdQuery query, CancellationToken ct)
    {
        if (await _mediator.Send(query, ct) is UserInteractionDto dto)
        {
            return Ok(dto);
        }
        return NotFound();
    }

    /// <summary>
    /// Patch UserInteraction model: change <c>IsOpen</c> state.
    /// </summary>
    /// <remarks>Patching is constrainted to set <c>IsOpen</c> property only,
    /// other props are ignored even if sent.
    /// </remarks>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(CUDOperationsExceptionFilter))]
    public async Task<IActionResult> PatchUserInteraction(
        Guid id,
        [FromHeader(Name = "If-Match")] byte[] rowVer,
        UserInteractionSetOpenStateCommand command,
        CancellationToken ct)
    {
        if (id != command.Id)
        {
        return BadRequest();
        }

        byte[] eTag = await _mediator.Send(command with { RowVer = rowVer }, ct);

        return new HeaderedStatusCodeResult(
            StatusCodes.Status204NoContent,
            new(HeaderNames.ETag, new[] { Convert.ToBase64String(eTag) }));
    }

    /// <summary>
    /// Put/update UserInteraction model.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(CUDOperationsExceptionFilter))]
    public async Task<IActionResult> PutUserInteraction(Guid id, UserInteractionUpdateCommand command, CancellationToken ct)
    {
        if (id == command.Id && await _mediator.Send(command, ct) == default)
        {
            return NoContent();
        }
        return BadRequest();
    }

    /// <summary>
    /// Create Userinteraction.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ServiceFilter(typeof(CUDOperationsExceptionFilter))]
    public async Task<ActionResult<UserInteractionDto>> PostUserInteraction(UserInteractionCreateCommand command, CancellationToken ct)
    {
        UserInteractionDto dto = await _mediator.Send(command, ct);

        return CreatedAtAction(nameof(GetUserInteraction), new { id = dto.Id }, dto);
    }
}
