using System;
using System.Linq;
using Backend.WebApi.Model;

namespace Backend.WebApi.Tests.TestInfrastructure;

public static class UserInteractionUtilities
{
    /// <summary>
    /// Generate test data to database, that can be requested from API tests.
    /// </summary>
    /// <remarks>
    /// Will use one-time DbContext to not to conflict with context used for testing.
    /// </remarks>
    /// <param name="dbFixture"></param>
    public static void SeedData(ApiDbContextLocalDbFixture dbFixture, params (Guid Id, bool IsOpen)[] knownEntityIds)
    {
        if (!knownEntityIds.Any())
        {
            throw new ArgumentException("Lähteandmed on nõutud entity'te genereerimiseks", nameof(knownEntityIds));
        }

        var entities = knownEntityIds.Select(known =>
            new UserInteraction()
            {
                Id = known.Id,
                Created = DateTime.Now,
                Deadline = DateTime.Now.AddDays(1),
                Description = TestNameGenerator(known.Id),
                IsOpen = known.IsOpen,
            });

        using var context = dbFixture.CreateContext();

        context.UserInteraction.AddRange(entities);

        context.SaveChanges();
    }

    private static string TestNameGenerator(Guid knownId) =>
        $@"Test entity to test {knownId.ToString()[..8]}";
}
