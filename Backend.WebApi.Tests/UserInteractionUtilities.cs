using System;
using System.Linq;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;

namespace Backend.WebApi.Tests;

public static class UserInteractionUtilities
{
    /// <summary>
    /// Generate test data to database, that can be requested from API tests.
    /// </summary>
    /// <remarks>
    /// Will use one-time DbContext to not to conflict with context used for testing.
    /// </remarks>
    /// <param name="dbFixture"></param>
    public static void SeedData(ApiLocalDbFixture dbFixture, params (Guid Id, bool IsOpen)[] knownEntityIds)
    {
        if (!knownEntityIds.Any())
        {
            throw new ArgumentException("Basedata for entity creation is mandatory.", nameof(knownEntityIds));
        }

        UserInteraction[] entities = GenerateEntities(knownEntityIds);

        using ApiDbContext context = dbFixture.CreateContext();

        context.UserInteraction.AddRange(entities);

        context.SaveChanges();
    }

    private static UserInteraction[] GenerateEntities((Guid Id, bool IsOpen)[] knownEntityIds)
    {
        var entities = knownEntityIds.Select(known =>
                    new UserInteraction()
                    {
                        Id = known.Id,
                        Created = DateTime.Now,
                        Deadline = DateTime.Now.AddDays(1),
                        Description = TestNameGenerator(known.Id),
                        IsOpen = known.IsOpen,
                    });

        return entities.ToArray();
    }

    private static string TestNameGenerator(Guid knownId) => $@"Test entity to test {knownId.ToString()[..8]}";
}
