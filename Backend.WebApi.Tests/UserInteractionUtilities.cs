using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;

namespace Backend.WebApi.Tests;

public static class UserInteractionUtilities
{
    /// <summary>
    /// Generates array of named tuples of Guid and bool. The represent UserInteraction entity's <see cref="UserInteraction.Id"/> and <see cref="UserInteraction.IsOpen"/> known values for testing.
    /// </summary>
    /// <remarks>
    /// Default implementation for IsOpen value will be <see langword="true"/> for every even element of returned array.
    /// </remarks>
    /// <param name="count">Count of elements to generate.</param>
    /// <returns></returns>
    public static (Guid Id, bool IsOpen)[] GenerateKnownData(int count = 4)
    {
        return Enumerable.Range(0, count).Select(i => (Guid.NewGuid(), i % 2 == 0)).ToArray();
    }

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
