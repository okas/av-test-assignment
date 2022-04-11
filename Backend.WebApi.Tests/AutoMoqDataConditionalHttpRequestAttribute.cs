using AutoFixture;
using AutoFixture.Kernel;
using Backend.WebApi.App.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.WebApi.Tests;

public class AutoMoqDataConditionalHttpRequestAttribute : AutoMoqDataAttribute
{
    public AutoMoqDataConditionalHttpRequestAttribute(bool omitAutoProperties = false)
        : base(ConfigureFixture(GetFixtureFactory(omitAutoProperties)()))
    { }

    private static Func<IFixture> ConfigureFixture(IFixture fixture) => () =>
    {
        fixture.Customizations.Add(new TypeRelay(typeof(IMemoryCache), typeof(MemoryCache)));
        fixture.Customizations.Add(new TypeRelay(typeof(ICacheService<>), typeof(MemoryCacheService<>)));

        return fixture;
    };
}
