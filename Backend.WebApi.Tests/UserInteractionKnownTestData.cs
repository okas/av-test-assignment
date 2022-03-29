namespace Backend.WebApi.Tests;

public readonly record struct UserInteractionKnownTestData(
    Guid Id,
    bool IsOpen,
    byte[] RowVer
);
