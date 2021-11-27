using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using Backend.WebApi.MapperExtensions;
using Backend.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.Controllers;

/// <summary>
/// Endpoint of Userinteractions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserInteractionsController : ControllerBase
{
    private readonly ApiDbContext _context;

    public UserInteractionsController(ApiDbContext context) => _context = context;

    /// <summary>
    /// Get all Userinteractions.
    /// </summary>
    /// <returns>Collection of all interactions.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserInteractionDto>>> GetAllUserInteractions()
    {
        return await _context.UserInteraction.AsNoTracking()
            .Select(model => model.ToDto())
            .ToListAsync();
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
        var model = await _context.UserInteraction.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        // TODO describe all result types for API
        return model.ToDto();
    }

    /// <summary>
    /// Update Userinteraction.
    /// </summary>
    /// <remarks>It do not allow fully to replace entity on given URI;
    /// `Created` property is protected, because it cannot be updated by user.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutUserInteraction(Guid id, UserInteractionUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }
        // TODO might need .Created property protection?
        _context.Entry(dto.ToModel()).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserInteractionExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return NoContent();
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

        var partialModel = new UserInteraction { Id = id, IsOpen = isOpenDto.IsOpen };
        _context.Attach(partialModel).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserInteractionExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        // TODO describe all result types for API
        return NoContent();
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
        // Map to model and add to DbContext, return reference to model.
        var model = _context.UserInteraction.Add(newDto.ToModel()).Entity;

        // These values setup is system's responsibility, thus doing it here.
        model.IsOpen = true;
        model.Created = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return HandleDbUpdateException(ex);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Some unexpected error occured. Sorry.");
        }

        return CreatedAtAction(nameof(GetUserInteraction), model.ToDto());
    }

    private bool UserInteractionExists(Guid id)
    {
        return _context.UserInteraction.Any(model => model.Id == id);
    }

    private ActionResult<UserInteractionDto> HandleDbUpdateException(DbUpdateException ex)
    {
        var errorMessage = ex.InnerException.Message.Contains("duplicat", StringComparison.InvariantCultureIgnoreCase)
            ? "Attempted to insert duplicate contract: only one Contract can exists between Company and User."
            : ex.InnerException.Message;

        ModelState.AddModelError("", errorMessage);
        return BadRequest(ModelState);
    }
}
