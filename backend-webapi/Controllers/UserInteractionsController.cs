using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.WebApi.Data.EF;
using Backend.WebApi.MapperExtensions;
using Backend.WebApi.Dto;
using Backend.WebApi.Model;

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
    /// Get all Userinteractions
    /// </summary>
    /// <returns>Collection of all interactions.</returns>
    [HttpGet]
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
    /// <param name="id"></param>
    /// <param name="dto"></param>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserInteraction(Guid id, UserInteractionDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        _context.Entry(dto.ToModel()).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserInteractionExists(id))
                return NotFound();
            else
                throw;
        }
        // TODO describe all result types for API
        return NoContent();
    }

    /// <summary>
    /// Create Userinteraction.
    /// </summary>
    /// <param name="newDto"></param>
    [HttpPost]

    public async Task<ActionResult<UserInteractionDto>> PostUserInteraction(UserInteractionNewDto newDto)
    {
        var model = _context.UserInteraction.Add(newDto.ToModel()).Entity;
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

        return CreatedAtAction("GetUserInteraction", new { id = model.Id }, model.ToDto());
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
