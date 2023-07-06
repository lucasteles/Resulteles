using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

#pragma warning disable CS1591

namespace Resulteles;

using ResultT = Result<Success, string>;

/// <inheritdoc />
public class ResultSimpleAssertions
    : ReferenceTypeAssertions<ResultT, ResultSimpleAssertions>
{
    /// <inheritdoc />
    protected override string Identifier => "Result";

    /// <inheritdoc />
    public ResultSimpleAssertions(ResultT subject) : base(subject) { }

    public AndConstraint<ResultSimpleAssertions> BeOk()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(status => status.IsOk)
            .FailWith(
                "Expected {context:Result} to be Ok, but found Error {0}.",
                sub => sub.Value);

        return new(this);
    }

    public AndConstraint<ResultSimpleAssertions> BeError()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(status => status.IsError)
            .FailWith("Expected {context:Result} to be Error, but found Ok.");
        return new(this);
    }

    public AndConstraint<ResultSimpleAssertions> HaveMessage(string message)
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(status => status.Value is string err && err == message)
            .FailWith("Expected {context:Result} to have message {0}, but found {1}.",
                _ => message, sub => sub.Value);

        return new(this);
    }

    public AndConstraint<ResultSimpleAssertions> BeErrorWithMessage(string message) =>
        BeError().And.HaveMessage(message);
}

/// <inheritdoc />
public class ResultTStringAssertions<T>
    : ReferenceTypeAssertions<Result<T, string>, ResultTStringAssertions<T>>
{
    /// <inheritdoc />
    protected override string Identifier => "ResultT";

    readonly ResultSimpleAssertions baseSimpleAssertion;

    /// <inheritdoc />
    public ResultTStringAssertions(Result<T, string> subject) : base(subject) =>
        baseSimpleAssertion = new(subject.Select(_ => new Success()));


    public AndConstraint<ResultTStringAssertions<T>> BeOk()
    {
        baseSimpleAssertion.BeOk();
        return new(this);
    }

    public AndConstraint<ResultTStringAssertions<T>> BeError()
    {
        baseSimpleAssertion.BeError();
        return new(this);
    }

    public AndConstraint<ResultTStringAssertions<T>> HaveMessage(string message)
    {
        baseSimpleAssertion.HaveMessage(message);
        return new(this);
    }

    public AndConstraint<ResultTStringAssertions<T>> ErrorWithMessage(string message) =>
        BeError().And.HaveMessage(message);

    public AndConstraint<ResultTStringAssertions<T>> Be(T other)
    {
        Subject.Value.Should().Be(other);
        return new(this);
    }

    public AndConstraint<ResultTStringAssertions<T>> BeOkThen(Action<T> action)
    {
        baseSimpleAssertion.BeOk();
        action((T)Subject.Value);
        return new(this);
    }

    public AndConstraint<ResultTStringAssertions<T>> BeEquivalentTo<TOther>(TOther other)
    {
        Subject.Value.Should().BeEquivalentTo(other);
        return new(this);
    }

    public AndConstraint<ResultTStringAssertions<T>> BeEquivalentTo<TOther>(TOther other,
        Func<EquivalencyAssertionOptions<TOther>, EquivalencyAssertionOptions<TOther>> options)
    {
        Subject.Value.Should().BeEquivalentTo(other, options);
        return new(this);
    }
}

/// <inheritdoc />
public class ResultTypeAssertions<TOk, TError>
    : ReferenceTypeAssertions<Result<TOk, TError>, ResultTypeAssertions<TOk, TError>>
{
    /// <inheritdoc />
    protected override string Identifier => "ResultType";

    /// <inheritdoc />
    public ResultTypeAssertions(Result<TOk, TError> subject) : base(subject) { }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeOk()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(status => status.IsOk)
            .FailWith(
                "Expected {context:Result} to be Ok, but found Error {0}.",
                sub => sub.Value);

        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeOk(TOk okValue)
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(sub => sub.IsOk && sub.Value.Equals(okValue))
            .FailWith(
                "Expected {context:Result} to be Ok with value {0}, but found Error {1}.",
                _ => okValue, sub => sub.Value
            );

        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeError()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(status => status.IsError)
            .FailWith("Expected {context:Result} to be Error, but found Ok.");
        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeError(TError errorValue)
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(sub => sub.IsError && sub.Value.Equals(errorValue))
            .FailWith("Expected {context:Result} to be Error {0}, but found Ok {1}",
                _ => errorValue, s => s.Value
            );
        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeEquivalentTo<TOther>(TOther other)
    {
        Subject.Value.Should().BeEquivalentTo(other);
        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeEquivalentTo<TOther>(TOther other,
        Func<EquivalencyAssertionOptions<TOther>, EquivalencyAssertionOptions<TOther>> options)
    {
        Subject.Value.Should().BeEquivalentTo(other, options);
        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeOkThen(Action<TOk> action)
    {
        BeOk();
        action((TOk)Subject.Value);
        return new(this);
    }

    public AndConstraint<ResultTypeAssertions<TOk, TError>> BeErrorThen(Action<TError> action)
    {
        BeError();
        action((TError)Subject.Value);
        return new(this);
    }
}
