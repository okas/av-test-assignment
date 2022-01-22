using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Backend.WebApi.Tests;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute(bool omitAutoProperties = false)
        : base(GetFixtureFactory(omitAutoProperties))
    { }

    private static Func<IFixture> GetFixtureFactory(bool omitAutoProperties) => () =>
    {
        IFixture fixture = new Fixture() { OmitAutoProperties = omitAutoProperties };

        fixture.Customize(new AutoMoqCustomization());

        return fixture;
    };
}
