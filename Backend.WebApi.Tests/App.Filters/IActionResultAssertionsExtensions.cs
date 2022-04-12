using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Backend.WebApi.Tests.App.Filters;

public static class IActionResultAssertionsExtensions
{
    public static FilterContextAssertions Should(this IActionResult? subject) => new(subject);

    public class FilterContextAssertions : ReferenceTypeAssertions<IActionResult?, FilterContextAssertions>
    {
        public FilterContextAssertions(IActionResult? subject) : base(subject) { }

        [CustomAssertion]
        public AndWhichConstraint<FilterContextAssertions, IActionResult> BeStatusCodeResultHttp304()
        {
            using AssertionScope _ = new();

            Subject.Should().BeOfType<StatusCodeResult>();

            Subject.As<IStatusCodeActionResult>().StatusCode.Should().Be(StatusCodes.Status304NotModified);

            return new(this, Subject!);
        }

        [CustomAssertion]
        public AndWhichConstraint<FilterContextAssertions, IActionResult> NotBeStatusCodeResultHttp304()
        {
            using AssertionScope _ = new();

            Subject.Should().NotBeOfType<StatusCodeResult>();

            Subject.As<IStatusCodeActionResult>().StatusCode!.Should().NotBe(304);

            return new(this, Subject!);
        }

        protected override string Identifier { get; } = "action result context";
    }
}
