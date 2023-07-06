using System.Text.Json;

namespace Resulteles.Tests;

public class ResultTests
{
    [Test]
    public void ShouldCompareEqualBeTrue()
    {
        var ok1 = Result<int, string>.Ok(42);
        var ok2 = Result<int, string>.Ok(42);
        (ok1 == ok2).Should().BeTrue();
    }

    [Test]
    public void ShouldCompareEqualBeFalse()
    {
        var ok1 = Result<int, string>.Ok(42);
        var ok2 = Result<int, string>.Ok(99);
        (ok1 == ok2).Should().BeFalse();
    }

    [Test]
    public void ShouldCompareNotEqualBeFalse()
    {
        var ok1 = Result<int, string>.Ok(42);
        var ok2 = Result<int, string>.Ok(42);
        (ok1 != ok2).Should().BeFalse();
    }

    [Test]
    public void ShouldCompareNotEqualBeTrue()
    {
        var ok1 = Result<int, string>.Ok(42);
        var ok2 = Result<int, string>.Ok(99);
        (ok1 != ok2).Should().BeTrue();
    }

    [Test]
    public void ShouldSerializeOkResult()
    {
        var ok = Result<int, string>.Ok(42);
        var strJson = JsonSerializer.Serialize(ok);
        var okFromJson = JsonSerializer.Deserialize<Result<int, string>>(strJson);
        okFromJson.Should().Be(ok);
    }

    [Test]
    public void ShouldSerializeErrorResult()
    {
        var error = Result<int, string>.Error("foo");
        var strJson = JsonSerializer.Serialize(error);
        var errorFromJson = JsonSerializer.Deserialize<Result<int, string>>(strJson);
        errorFromJson.Should().Be(error);
    }

    [Test]
    public void ShouldPatternMatchPropertyOk()
    {
        if (Result<int, string>.Ok(42) is { Value: 42 })
            Assert.Pass();

        Assert.Fail("unexpected!");
    }

    [Test]
    public void ShouldPatternMatchTupleOk()
    {
        if (Result<int, string>.Ok(42) is (true, 42, null))
            Assert.Pass();

        Assert.Fail("unexpected!");
    }

    [Test]
    public void ShouldTryOk()
    {
        var result = Result<int, string>.Ok(42);

        if (result.TryOk(out var value) && value == 42)
            Assert.Pass();

        Assert.Fail();
    }

    [Test]
    public void ShouldTryError()
    {
        var result = Result<int, string>.Error("Failure");

        if (result.TryError(out var value) && value == "Failure")
            Assert.Pass();

        Assert.Fail();
    }

    [Test]
    public void ShouldTryGetOrError()
    {
        var result = Result<int, string>.Error("BAD");

        if (!result.AsNullable().TryGet(out var value, out var error))
        {
            Assert.IsNull(value);
            Assert.IsNotNull(error);
            Assert.Pass();
        }

        Assert.Fail();
    }

    [Test]
    public void ShouldMatchSuccessValue()
    {
        var result = Result<int, string>.Ok(42);

        var value = result.Match(
            x => x.ToString(),
            _ => "NOPE"
        );

        value.Should().Be("42");
    }

    [Test]
    public void ShouldMatchErrorValue()
    {
        var result = Result<int, string>.Error("Err");

        var value = result.Match(
            x => x.ToString(),
            error => $"{error}!"
        );

        value.Should().Be("Err!");
    }


    [Test]
    public void ShouldSwitchOnSuccessValue()
    {
        var result = Result<int, string>.Ok(42);

        result.Switch(
            value =>
            {
                value.Should().Be(42);
                Assert.Pass();
            },
            _ =>
            {
                Assert.Fail();
            });

        Assert.Fail();
    }

    [Test]
    public void ShouldSwitchOnErrorValue()
    {
        var result = Result<int, string>.Error("Err");

        result.Switch(
            _ =>
            {
                Assert.Fail();
            },
            error =>
            {
                error.Should().Be("Err");
                Assert.Pass();
            });

        Assert.Fail();
    }

    [PropertyTest]
    public void ShouldValueBeSameAsOkValue(int okValue)
    {
        var result = Result.Ok(okValue);
        result.Value.Should().Be(okValue);
    }

    [PropertyTest]
    public void ShouldValueBeSameAsErrorValue(string errorValue)
    {
        var result = Result.Error(errorValue);
        result.Value.Should().Be(errorValue);
    }

    [PropertyTest]
    public bool ShouldExplicitCastOnOk(int okValue)
    {
        var result = Result.Ok(okValue);
        var casted = (int)result;

        return casted == okValue;
    }

    [PropertyTest]
    public bool ShouldExplicitCastOnError(string errorValue)
    {
        var result = Result.Error(errorValue);
        var casted = (string)result;

        return casted == errorValue;
    }


    [Test]
    public void ShouldThrowOnBadExplicitErrorCast()
    {
        var result = Result.Error<int, string>("NOPE");
        var action = () => (int)result;
        action.Should().Throw<ResultInvalidCastException>();
    }

    [Test]
    public void ShouldThrowOnExplicitCastOnError()
    {
        var result = Result.Ok<int, string>(42);
        var action = () => (string)result;
        action.Should().Throw<ResultInvalidCastException>();
    }

    [Test]
    public void ShouldReturnOkValueImplicitly()
    {
        Result<int, string> Stuff() => 42;
        Stuff().Should().Be(Result.Ok(42));
    }

    [Test]
    public void ShouldReturnErrorValueImplicitly()
    {
        Result<int, string> Stuff() => "Err";
        Stuff().Should().Be(Result.Error<int, string>("Err"));
    }


    [Test]
    public void ShouldTryResultWithSuccess()
    {
        var result = Result.Try(() => 42);
        result.Should().Be(Result.Ok<int, Exception>(42));
    }

    [Test]
    public void ShouldTryResultWithError()
    {
        var result = Result.Try<int>(() =>
        {
            throw new InvalidOperationException("NOPE");
        });

        result.Should().BeOfType<Result<int, Exception>>();
        result.Value.Should().BeOfType<InvalidOperationException>().And.BeEquivalentTo(new { Message = "NOPE" });
    }

    [Test]
    public async Task ShouldTryAsyncResultWithSuccess()
    {
        var result = await Result.TryAsync(() => Task.FromResult(42));
        result.Should().Be(Result.Ok<int, Exception>(42));
    }

    [Test]
    public async Task ShouldTryAsyncResultWithError()
    {
        var result = await Result.TryAsync<int>(() =>
        {
            throw new InvalidOperationException("NOPE");
        });

        result.Should().BeOfType<Result<int, Exception>>();
        result.Value.Should().BeOfType<InvalidOperationException>().And.BeEquivalentTo(new { Message = "NOPE" });
    }
}
