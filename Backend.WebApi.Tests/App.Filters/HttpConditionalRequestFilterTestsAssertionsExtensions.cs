using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.Tests.App.Filters;
public static class HttpConditionalRequestFilterTestsAssertionsExtensions
{
    public static IfNoneMatchAssertions Should(this ActionExecutingContext subject) => new(subject);

    public class IfNoneMatchAssertions : ReferenceTypeAssertions<ActionExecutingContext, IfNoneMatchAssertions>
    {
        public IfNoneMatchAssertions(ActionExecutingContext subject) : base(subject) { }

        /// <summary>
        /// Short-circuited request is when <see cref="ActionExecutingContext.Result"/> is
        /// <see cref="StatusCodeResult"/> and <c>HTTP status code</c> is <see cref="StatusCodes.Status304NotModified"/>.
        /// </summary>
        [CustomAssertion]
        public AndWhichConstraint<IfNoneMatchAssertions, ActionExecutingContext> BeShortCircuited(string eTag/*, string because = "", params object[] becauseArgs*/)
        {
            using AssertionScope _ = new();

            Execute.Assertion
                .ForCondition(!string.IsNullOrWhiteSpace(eTag))
                .FailWith("Cannot assert, if eTag is not passed!");

            Subject.Result.Should().BeStatusCodeResultHttp304();

            Subject.HttpContext.Response.Headers.ETag.Should().Contain(eTag);

            Subject.HttpContext.Response.Body.Length.Should().Be(0);

            return new(this, Subject!);
        }

        /// <summary>
        /// <inheritdoc cref="BeShortCircuited"/>
        /// <para>
        /// <see cref="ActionExecutingContext.Result"/> is allowed to be <see langword="null"/>, because filter itself
        /// should do anityhing with neither <see cref="ActionExecutingContext.Result"/>
        /// nor <see cref="ActionContext.HttpContext"/> contents in any way.
        /// </para>
        /// </summary>
        [CustomAssertion]
        public AndWhichConstraint<IfNoneMatchAssertions, ActionExecutingContext> NotBeShortCircuited()
        {
            using AssertionScope _ = new();

            if (Subject.Result is not null)
            {
                Subject.Result.Should().NotBeStatusCodeResultHttp304();
            }

            return new(this, Subject!);
        }

        protected override string Identifier { get; } = "action executing context";
    }
}
