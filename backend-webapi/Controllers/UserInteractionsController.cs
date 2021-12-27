using System.Linq.Expressions;
using Backend.WebApi.Dto;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

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
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserInteractionDto>>> GetUserInteractions(bool? isOpen = null)
    {
        Expression<Func<UserInteraction, bool>> filters = model => !isOpen.HasValue || model.IsOpen == isOpen;

        (IEnumerable<ServiceError> errors, IEnumerable<UserInteractionDto>? dtos, int totalCount) =
            await _service.Get(UserInteractionDto.Projection, filters);

        return Ok(dtos);
    }

    /// <summary>
    /// Get Userinteraction by ID.
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionDto>> GetUserInteraction(Guid id)
    {
        (_, UserInteraction? model) = await _service.GetOne(id);

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
    public async Task<IActionResult> PatchUserInteraction(Guid id, UserInteractionIsOpenDto isOpenDto)
    {
        if (id != isOpenDto.Id)
        {
            return BadRequest();
        }

        IEnumerable<ServiceError> errors = await _service.SetOpenState(id, isOpenDto.IsOpen);

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
    public async Task<ActionResult<UserInteractionDto>> PostUserInteraction(UserInteractionNewDto newDto)
    {
        (IEnumerable<ServiceError> errors, UserInteraction? model) = await _service.Create(new()
        {
            Description = newDto.Description,
            Deadline = newDto.Deadline,
        });

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

        throw exceptions?.First() ?? new Exception(_noExceptionInErrorMessage);
    }
}
