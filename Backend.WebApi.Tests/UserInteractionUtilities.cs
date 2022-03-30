using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;

namespace Backend.WebApi.Tests;

public static class UserInteractionUtilities
{
    public static Guid[] GenerateWithKnownId(int count = 3) => Enumerable.Range(0, count).Select(i => Guid.NewGuid()).ToArray();

    /// <summary>
    /// Generates array of named tuples of Guid and bool. The represent UserInteraction entity's <see cref="UserInteraction.Id"/> and <see cref="UserInteraction.IsOpen"/> known values for testing.
    /// </summary>
    /// <remarks>
    /// Default implementation for IsOpen value will be <see langword="true"/> for every even element of returned array.
    /// </remarks>
    /// <param name="count">Count of elements to generate.</param>
    public static (Guid Id, bool IsOpen)[] GenerateWithKnownIdIsOpen(int count = 4) => GenerateWithKnownId(count).ToIdIsOpen();

    public static void SeedData(ApiLocalDbFixture dbFixture, params Guid[] knownEntitesId) => SeedData(dbFixture, knownEntitesId.ToIdIsOpen());

    /// <summary>
    /// Generate test data to database, that can be requested from API tests, using knwon Id's and IsOpen values. Former according to algorithm of <see cref="GenerateWithKnownId"/>
    /// </summary>
    /// <param name="dbFixture"></param>
    /// <param name="count">Known data entries to generate and seed.</param>
    public static UserInteractionKnownTestData[] SeedDataGenerateAndReturnKnown(ApiLocalDbFixture dbFixture, int count = 3)
    {
        (Guid x, bool)[] knownEntityIds = GenerateWithKnownId(count).ToIdIsOpen();

        UserInteraction[] entities = GenerateEntities(knownEntityIds);

        using ApiDbContext context = dbFixture.CreateContext();

        context.UserInteraction.AddRange(entities);

        context.SaveChanges();

        return GenerateKnownUserInteractionTestData(knownEntityIds, context);

    }

    /// <summary>
    /// Generate test data to database, that can be requested from API tests.
    /// </summary>
    /// <remarks>
    /// Will use one-time DbContext to not to conflict with context used for testing.
    /// </remarks>
    public static UserInteractionKnownTestData[] SeedDataGenerateAndReturnKnown(ApiLocalDbFixture dbFixture, params (Guid Id, bool IsOpen)[] knownEntityIds)
    {
        if (!knownEntityIds.Any())
        {
            throw new ArgumentException("Basedata for entity creation is mandatory.", nameof(knownEntityIds));
        }

        UserInteraction[] entities = GenerateEntities(knownEntityIds);

        using ApiDbContext context = dbFixture.CreateContext();

        context.UserInteraction.AddRange(entities);

        context.SaveChanges();

        return GenerateKnownUserInteractionTestData(knownEntityIds, context);
    }

    /// <summary>
    /// Generate test data to database, that can be requested from API tests.
    /// </summary>
    /// <remarks>
    /// Will use one-time DbContext to not to conflict with context used for testing.
    /// </remarks>
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

    private static UserInteractionKnownTestData[] GenerateKnownUserInteractionTestData((Guid Id, bool IsOpen)[] knownEntityIds, ApiDbContext context)
    {
        Guid[] ids = knownEntityIds.Select(x => x.Id).ToArray();

        var withRowVer = context.UserInteraction
            .Where(e => ids.Contains(e.Id))
            .Select(e => new { e.Id, e.RowVer })
            .ToList();

        return knownEntityIds.Join(
            withRowVer,
            x => x.Id,
            y => y.Id,
            (x, y) => new UserInteractionKnownTestData(x.Id, x.IsOpen, y.RowVer))
            .ToArray();
    }

    private static (Guid x, bool)[] ToIdIsOpen(this Guid[] ids) => ids.Select((x, i) => (x, i % 2 == 0)).ToArray();

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
