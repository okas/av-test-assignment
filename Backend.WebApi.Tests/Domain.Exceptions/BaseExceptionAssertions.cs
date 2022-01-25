using Backend.WebApi.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;

namespace Backend.WebApi.Tests.Domain.Exceptions;

public static partial class BaseExceptionAssertions
{
    public static ExceptionAssertions Should(this BaseException subject) => new(subject);

    public static FuncAssertions<T> Should<T>(this Func<T> subject)
        where T : BaseException
        => new(subject);


    public class ExceptionAssertions : ReferenceTypeAssertions<BaseException, ExceptionAssertions>
    {
        public ExceptionAssertions(BaseException subject) : base(subject)
        { }

        [CustomAssertion]
        public AndConstraint<ExceptionAssertions> BeCorrectlyInitializedViaConstructor(string? message, object model, string? category)
        {
            using AssertionScope _ = new(nameof(BaseException));

            Subject.Message.Should().Be(message);
            Subject.Data[BaseException.ModelDataKey].Should().Be(model);
            Subject.Category.Should().Be(category);

            return new(this);
        }

        protected override string Identifier => "exception";
    }

    public class FuncAssertions<T> : FunctionAssertions<BaseException> where T : BaseException
    {
        public FuncAssertions(Func<BaseException> subject)
            : base(subject, Extractor)
        { }

        private static AggregateExceptionExtractor Extractor { get; } = new AggregateExceptionExtractor();

        [CustomAssertion]
        public AndConstraint<FuncAssertions<T>> ThrowOnBadCategory()
        {
            using AssertionScope _ = new();

            Subject.Should().ThrowExactly<ArgumentOutOfRangeException>()
                .WithMessage("*should not be null, empty, or whitespace string*");

            return new(this);
        }
    }
}
