using Backend.WebApi.App.Dto;

namespace Backend.WebApi.Tests.App.Extensions;

public record ETaggedStub : IETag
{
    public string ETag { get; init; }
}
