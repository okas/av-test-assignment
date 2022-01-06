using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

[StructLayout(LayoutKind.Auto)]
public readonly record struct UserInteractionSetOpenStateCommand(
    [property: Required, NotDefault] Guid Id,
    [property: Required] bool IsOpen
    )
    : IRequest
{
    public record Handler(ApiDbContext Context) : IRequestHandler<UserInteractionSetOpenStateCommand>
    {
        public const string NotFoundOnIsOpenChangeMessage = "User interaction not found, while attempting to set its Open state.";

        public async Task<Unit> Handle(UserInteractionSetOpenStateCommand rq, CancellationToken ct)
        {
            Context.Attach(new UserInteraction
            {
                Id = rq.Id,
                IsOpen = rq.IsOpen,
            }
            ).Property(model => model.IsOpen).IsModified = true;

            try
            {
                await Context.SaveChangesAsync(ct).ConfigureAwait(false);

                return Unit.Value;
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO Log it
                // TODO Analyze https://docs.microsoft.com/en-us/ef/core/saving/concurrency to implement better handling
                if (!await Context.UserInteraction.AnyAsync(model => model.Id == rq.Id, ct).ConfigureAwait(false))
                {
                    throw new NotFoundException(NotFoundOnIsOpenChangeMessage, rq.Id);
                }
                throw;
            }
            catch
            {
                // TODO Whether and what should be logged here?
                throw;
            }
        }
    }
}
