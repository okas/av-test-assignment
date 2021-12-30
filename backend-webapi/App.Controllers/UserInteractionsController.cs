using System.Linq.Expressions;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Services;
using Backend.WebApi.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.App.Controllers;

/// <summary>
/// Endpoint of Userinteractions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserInteractionsController : ControllerBase
{
    private static readonly string _noExceptionInErrorMessage;
    private readonly UserInteractionService _service;

    static UserInteractionsController()
    {
        _noExceptionInErrorMessage = $"For developer: {nameof(UserInteractionService)} returned error result, but exception is not found.";
    }

    public UserInteractionsController(UserInteractionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get User interactions. By default, all Interactions. Allows filtering by `IsOpen` property in query string.
    /// </summary>
    /// <param name="isOpen">Ommiting this parameter will return all User interactions.</param>
    /// <returns>All or filtered by `IsOpen` collection of interactions.</returns>
    /// <param name="ct"></param>
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserInteractionDto>>> GetUserInteractions(bool? isOpen, CancellationToken ct)
    {
        Expression<Func<UserInteraction, bool>> filters = model => !isOpen.HasValue || model.IsOpen == isOpen;

        (IEnumerable<ServiceError> errors, IEnumerable<UserInteractionDto>? dtos, int totalCount) =
            await _service.Get(ct, UserInteractionDto.Projection, filters);

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
        (_, UserInteraction? model) = await _service.GetOne(id, ct);

        if (model is not null)
        {
            return Ok(UserInteractionDto.Projection.Compile().Invoke(model));
        }

        return NotFound();
    }

    /// <summary>
    /// Patch UserInteraction model: change `IsOpen` state.
    /// </summary>
    /// <remarks>Patching is constrainted to set `IsOpen` property only,
    /// other props are ignored even if sent.
    /// </remarks>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchUserInteraction(Guid id, UserInteractionIsOpenDto isOpenDto, CancellationToken ct)
    {
        if (id != isOpenDto.Id)
        {
            return BadRequest();
        }

        IEnumerable<ServiceError> errors = await _service.SetOpenState(id, isOpenDto.IsOpen, ct);

        if (!errors.Any())
        {
            return NoContent();
        }

        if (errors.Any(err => err.Kind == ServiceErrorKind.NotFoundOnChange))
        {
            return NotFound();
        }

        (_, _, Exception?[]? exceptions) = errors.First(err => err.Exceptions?.Any() ?? false);

        throw exceptions?.First() ?? new Exception(_noExceptionInErrorMessage);
    }

    /// <summary>
    /// Create Userinteraction.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserInteractionDto>> PostUserInteraction(UserInteractionNewDto newDto, CancellationToken ct)
    {
        UserInteraction newModel = new()
        {
            Description = newDto.Description,
            Deadline = newDto.Deadline,
        };

        (IEnumerable<ServiceError> errors, UserInteraction? model) = await _service.Create(newModel, ct);

        if (!errors.Any() && model is not null)
        {
            UserInteractionDto dto = UserInteractionDto.Projection.Compile().Invoke(model);

            return CreatedAtAction(nameof(GetUserInteraction), new { id = model.Id }, dto);
        }

        if (errors.First(err => err.Kind == ServiceErrorKind.AlreadyExistsOnCreate) is ServiceError error1)
        {
            ModelState.AddModelError("", error1.Message ?? "Duplicate entity error.");

            return BadRequest(ModelState);
        }

        if (errors.First(err => err.Kind == ServiceErrorKind.InternalError) is ServiceError error2)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, error2.Message);
        }

        (_, _, Exception?[]? exceptions) = errors.First(err => err.Exceptions?.Any() ?? false);

        throw new AggregateException("Multiple internal exceptions thrown.", exceptions) ?? new Exception(_noExceptionInErrorMessage);
    }
}
