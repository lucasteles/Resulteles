namespace Resulteles.Tests;

public class ResultValueTests
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message) { }
    }

    [Test]
    public void ShouldGetValueOrThrowReturnValueWhenIsOk()
    {
        var result = Result<int, string>.Ok(42);
        var value = result.GetValueOrThrow();
        value.Should().Be(42);
    }

    [Test]
    public void ShouldGetValueOrThrowRaiseAnExceptionWhenIsError()
    {
        var result = Result<int, string>.Error("Err");
        var action = () => result.GetValueOrThrow();
        action.Should().Throw<ResultException>().WithMessage("Err");
    }

    [Test]
    public void ShouldGetValueOrThrowRaiseAFormattedExceptionWhenIsError()
    {
        var result = Result<int, string>.Error("Err");
        var action = () => result.GetValueOrThrow(e => e.ToUpper());
        action.Should().Throw<ResultException>().WithMessage("ERR");
    }

    [Test]
    public void ShouldGetValueOrThrowRaiseACustomExceptionWhenIsError()
    {
        var result = Result<int, string>.Error("Err");
        var action = () => result.GetValueOrThrow(e => new TestException(e));
        action.Should().Throw<TestException>().WithMessage("Err");
    }

    [Test]
    public void ShouldThrowIfErrorRaiseExceptionOnError()
    {
        var result = Result<int, string>.Error("Err");
        var action = () => result.ThrowIfError();
        action.Should().Throw<ResultException>().WithMessage("Err");
    }

    [Test]
    public void ShouldThrowIfErrorRaiseFormattedExceptionOnError()
    {
        var result = Result<int, string>.Error("Err");
        var action = () => result.ThrowIfError(e => e.ToUpper());
        action.Should().Throw<ResultException>().WithMessage("ERR");
    }

    [Test]
    public void ShouldThrowIfErrorRaiseCustomExceptionOnError()
    {
        var result = Result<int, string>.Error("Err");
        var action = () => result.ThrowIfError(e => new TestException(e));
        action.Should().Throw<TestException>().WithMessage("ERR");
    }

    [Test]
    public void ShouldReturnOkValueWhenDefaultValueOnOkResult()
    {
        var result = Result<int, string>.Ok(42).DefaultValue(1);
        result.Should().Be(42);
    }

    [Test]
    public void ShouldReturnDefaultValueWhenDefaultValueOnErrorResult()
    {
        var result = Result<int, string>.Error("Error").DefaultValue(1);
        result.Should().Be(1);
    }

    [Test]
    public void ShouldReturnOkValueWhenDefautlWithOnOkResult()
    {
        var result = Result<int, string>.Ok(42).DefaultWith(_ => 1);
        result.Should().Be(42);
    }

    [Test]
    public void ShouldReturnDefaultValueWhenDefaultWithOnErrorResult()
    {
        var result = Result<int, string>.Error("1").DefaultWith(int.Parse);
        result.Should().Be(1);
    }

    [Test]
    public void ShouldReturnNullableValueTypeOkToNullable()
    {
        var result = Result<int, string>.Ok(42).ToNullable();
        result.Should().Be(new int?(42));
    }

    [Test]
    public void ShouldReturnNullableValueTypeErrorToNullable()
    {
        var result = Result<int, string>.Error("").ToNullable();
        result.Should().Be(null);
    }

    [Test]
    public void ShouldTapValue()
    {
        var tappedValue = 0;
        Result<int, string>.Ok(42).Tap(v => tappedValue = v);
        tappedValue.Should().Be(42);
    }
}
