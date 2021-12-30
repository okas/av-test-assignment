﻿using Backend.WebApi.App.Services;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public class UserInteractionCreateHandler : IRequestHandler<UserInteractionCreateCommand, (IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    private readonly ApiDbContext _context;
    private static readonly string _createNewModelErrorMessage;

    static UserInteractionCreateHandler() =>
        _createNewModelErrorMessage = $"Attempted to create new `{nameof(UserInteraction)}`, but operation was cancelled unexpectedly. See excpetion details.";

    public UserInteractionCreateHandler(ApiDbContext dbContext) => _context = dbContext;

    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> Handle(
        UserInteractionCreateCommand request,
        CancellationToken ct)
    {
        UserInteraction model = new()
        {
            IsOpen = true,
            Created = DateTime.Now,
            Description = request.Description,
            Deadline = request.Deadline,
        };

        try
        {
            await _context.UserInteraction.AddAsync(model, ct);
            await _context.SaveChangesAsync(ct);

            return (Enumerable.Empty<ServiceError>(), model);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.AlreadyExistsOnCreate) };

            return (errors, model: default);
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };

            return (errors, model: default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            ServiceError[] errors = new ServiceError[] { new(ServiceErrorKind.InternalError, _createNewModelErrorMessage, ex) };

            return (errors, model: default);
        }
    }
}
