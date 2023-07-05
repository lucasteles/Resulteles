using System.Text.Json;
using Resulteles;

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
    public void ShouldCombineOkResults()
    {
        var result =
            from ok1 in Result<int, string>.Ok(42)
            from ok2 in Result<int, string>.Ok(100)
            select ok1 + ok2;

        result.Should().Be(Result.Ok(142));
    }

    [Test]
    public void ShouldShorCircuitErrorResult()
    {
        var result =
            from ok1 in Result<int, string>.Ok(42)
            from ok2 in Result<int, string>.Error("FAIL")
            from ok3 in Result<int, string>.Ok(100)
            select ok1 + ok2 + ok3;
        result.Should().Be(Result.Error<int, string>("FAIL"));
    }

    [Test]
    public void ShouldBeEnumerable()
    {
        foreach (var value in Result<int, string>.Ok(42).AsEnumerable())
            if (value == 42)
                Assert.Pass();

        Assert.Fail("unexpected!");
    }

    [Test]
    public void ShouldMatchPropertyOk()
    {
        if (Result<int, string>.Ok(42) is { Value: 42 })
            Assert.Pass();

        Assert.Fail("unexpected!");
    }

    [Test]
    public void ShouldMatchTupleOk()
    {
        if (Result<int, string>.Ok(42) is (true, 42, null))
            Assert.Pass();

        Assert.Fail("unexpected!");
    }

    [Test]
    public void ShouldMapValue() =>
        Result<int, string>.Ok(42)
            .Select(x => x.ToString())
            .Should().Be(Result<string, string>.Ok("42"));

    [Test]
    public void ShouldTryGet()
    {
        var result = Result<int, string>.Ok(42);

        if (result.TryOk(out var value) && value == 42)
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
    public async Task ShouldMapAsync()
    {
        var result = await Result<int, string>.Ok(42).SelectAsync(Task.FromResult);
        result.Should().Be(Result.Ok(42));
    }

    [Test]
    public async Task ShouldBindAsync()
    {
        var result = await Result<int, string>.Ok(42)
            .SelectManyAsync(x => Task.FromResult(Result.Ok(x + 10)));

        result.Should().Be(Result.Ok(52));
    }
}
