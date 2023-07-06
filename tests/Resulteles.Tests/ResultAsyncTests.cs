#pragma warning disable CS1998
namespace Resulteles.Tests;

public class ResultAsyncTests
{
    [Test]
    public async Task ShouldMapAsync()
    {
        var result = await Result<int, string>.Ok(42).SelectAsync(Task.FromResult);
        result.Should().Be(Result.Ok(42));
    }

    [Test]
    public async Task ShouldMapOkAndErrorsOnOkAsync()
    {
        var result = await Result<int, string>.Ok(42).SelectAsync(Task.FromResult, Task.FromResult);
        result.Should().Be(Result.Ok(42));
    }

    [Test]
    public async Task ShouldMapOkAndErrorsOnErrorAsync()
    {
        var result = await Result<int, string>.Error("Err").SelectAsync(Task.FromResult, Task.FromResult);
        result.Should().Be(Result.Error<int, string>("Err"));
    }


    [Test]
    public async Task ShouldBindAsync()
    {
        var result = await Result<int, string>.Ok(42)
            .SelectManyAsync(x => Task.FromResult(Result.Ok(x + 10)));

        result.Should().Be(Result.Ok(52));
    }

    [Test]
    public async Task ShouldTapValue()
    {
        var tappedValue = 0;
        _ = await new Result<int, string>(42).Tap(async v => tappedValue = v);
        tappedValue.Should().Be(42);
    }

    [Test]
    public async Task ShouldMatchSuccessValueAsync()
    {
        var result = Result<int, string>.Ok(42);

        var value = await result.Match(
            async x => x.ToString(),
            _ => ""
        );

        value.Should().Be("42");
    }

    [Test]
    public async Task ShouldMatchErrorValue()
    {
        var result = Result<int, string>.Error("Err");

        var value = await result.Match(
            x => x.ToString(),
            async error => $"{error}!"
        );

        value.Should().Be("Err!");
    }

    [Test]
    public async Task ShouldMatchSuccessValueBothAsync()
    {
        var result = Result<int, string>.Ok(42);

        var value = await result.Match(
            async x => x.ToString(),
            async _ => ""
        );

        value.Should().Be("42");
    }

    [Test]
    public async Task ShouldMatchErrorBothValue()
    {
        var result = Result<int, string>.Error("Err");

        var value = await result.Match(
            async x => x.ToString(),
            async error => $"{error}!"
        );

        value.Should().Be("Err!");
    }

    [Test]
    public async Task ShouldSwitchOnSuccessValueAsync()
    {
        var result = Result<int, string>.Ok(42);

        await result.SwitchAsync(
            async value =>
            {
                value.Should().Be(42);
                Assert.Pass();
            },
            async _ =>
            {
                Assert.Fail();
            });

        Assert.Fail();
    }

    [Test]
    public async Task ShouldSwitchOnErrorValueAsync()
    {
        var result = Result<int, string>.Error("Err");

        await result.SwitchAsync(
            async _ =>
            {
                Assert.Fail();
            },
            async error =>
            {
                error.Should().Be("Err");
                Assert.Pass();
            });

        Assert.Fail();
    }
}
