using Backend.WebApi.Dto;
using Backend.WebApi.MapperExtensions;
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
    private readonly UserInteractionService _service;

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
        var (errors, dtos, totalCount) = await _service.GetSome(
               model => model.ToDto(),
               model => !isOpen.HasValue || model.IsOpen == isOpen
               );

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
        var (_, model) = await _service.GetOne(id);
        if (model is null)
        {
            return NotFound();
        }
        // TODO describe all result types for API
        return Ok(model.ToDto());
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

        //var partialModel = new UserInteraction { Id = id, IsOpen = isOpenDto.IsOpen };
        //_context.Attach(partialModel).Property(model => model.IsOpen).IsModified = true;

        var errors = await _service.SetOpenState(id, isOpenDto.IsOpen);

        if (errors is null || !errors.Any())
        {
            return NoContent();
        }

        if (errors.Any(err => err.Kind == ServiceErrorKind.NotFoundOnChange))
        {
            return NotFound();
        }

        var (_, _, exceptions) = errors.First(err => err.Exceptions?.Any() ?? false);

        throw exceptions.First();
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
        var (errors, model) = await _service.Create(newDto.ToModel());

        if (errors is null || !errors.Any())
        {
            return CreatedAtAction(nameof(GetUserInteraction), new { id = model.Id }, model.ToDto());
        }

        ServiceError error;
        error = errors.First(err => err.Kind == ServiceErrorKind.AlreadyExistsOnCreate);
        if (error is not null)
        {
            ModelState.AddModelError("", error.Message ?? "Duplicate entity error.");
            return BadRequest(ModelState);
        }

        error = errors.First(err => err.Kind == ServiceErrorKind.InternalError);
        if (error is not null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, error.Message);
        }

        var (_, _, exceptions) = errors.First(err => err.Exceptions?.Any() ?? false);

        throw exceptions.First();
    }
}
