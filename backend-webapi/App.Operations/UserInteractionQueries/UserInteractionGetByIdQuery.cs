using System.ComponentModel.DataAnnotations;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.Domain.Model;
using MediatR;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public readonly record struct UserInteractionGetByIdQuery([property: Required, NotDefault] Guid Id) : IRequest<UserInteraction>;
