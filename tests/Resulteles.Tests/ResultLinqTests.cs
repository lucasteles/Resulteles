namespace Resulteles.Tests;

public class ResultLinqTests
{
    static T Identity<T>(T value) => value;

    static Result<int, string> IsEven(int n) => n % 2 == 0 ? n : $"Invalid {n}";


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
    public void ShouldAsEnumerableHaveOneItemOnOk()
    {
        var result = Result.Ok(42);
        result.AsEnumerable().Should().BeEquivalentTo(new[] { 42 });
    }

    [Test]
    public void ShouldAsEnumerableBeEmptyOnError()
    {
        var result = Result.Error("fail");
        result.AsEnumerable().Should().BeEmpty();
    }

    [Test]
    public void ShouldAToArrayHaveOneItemOnOk()
    {
        var result = Result.Ok(42);
        result.ToArray().Should().BeEquivalentTo(new[] { 42 });
    }

    [Test]
    public void ShouldArrayBeEmptyOnError()
    {
        var result = Result.Error("fail");
        result.ToArray().Should().BeEmpty();
    }

    [Test]
    public void ShouldMapOkValue() =>
        Result<int, string>.Ok(42)
            .Select(x => x.ToString())
            .Should().Be(Result<string, string>.Ok("42"));


    [Test]
    public void ShouldMapErrorValue() =>
        Result<int, long>.Error(-1)
            .SelectError(x => x.ToString())
            .Should().Be(Result<int, string>.Error("-1"));

    [Test]
    public void ShouldMapOkAndErrorValuesOnOk() =>
        Result<int, long>.Ok(42)
            .Select(x => x.ToString(), y => y.ToString())
            .Should().Be(Result<string, string>.Ok("42"));

    [Test]
    public void ShouldMapOkAndErrorValuesOnError() =>
        Result<int, long>.Error(-1)
            .Select(x => x.ToString(), y => y.ToString())
            .Should().Be(Result<string, string>.Error("-1"));

    [PropertyTest]
    public bool ShouldRespectFunctorIdentityLaw(Result<int, string> result)
    {
        var mapped = result.Select(Identity);
        return result == mapped;
    }

    [PropertyTest]
    public bool ShouldRespectFunctorCompositionLaw(Result<int, string> result, Func<int, long> f, Func<long, int> g)
    {
        var composed = result.Select(v => g(f(v)));
        var mapped = result.Select(f).Select(g);

        return composed == mapped;
    }

    [PropertyTest]
    public bool ShouldRespectMonadLeftIdentityLaw(int value, Func<int, Result<long, string>> h)
    {
        var mapped = Result.Ok(value).SelectMany(h);
        return mapped == h(value);
    }

    [PropertyTest]
    public bool ShouldRespectMonadRightIdentityLaw(Result<int, string> m)
    {
        var mapped = m.SelectMany(Result.Ok);
        return mapped == m;
    }

    [PropertyTest]
    public bool ShouldRespectMonadAssociativityIdentityLaw(
        Result<int, string> m,
        Func<int, Result<long, string>> g,
        Func<long, Result<string, string>> h
    )
    {
        var left = m.SelectMany(g).SelectMany(h);
        var right = m.SelectMany(x => g(x).SelectMany(h));

        return left == right;
    }

    [PropertyTest]
    public bool ShouldPropagateOkToString(int value) =>
        Result.Ok(value).ToString() == value.ToString();

    [PropertyTest]
    public bool ShouldPropagateErrorToString(int value) =>
        Result.Error(value).ToString() == value.ToString();

    [Test]
    public void ShouldZipValuesIntoTupleOnOk()
    {
        var r1 = Result.Ok(42);
        var r2 = Result.Ok(1);

        var result = r1.Zip(r2);
        result.Should().Be(Result<(int, int), string>.Ok((42, 1)));
    }

    [Test]
    public void ShouldZipValuesWithFunction()
    {
        var r1 = Result.Ok(42);
        var r2 = Result.Ok(1);

        var result = r1.Zip(r2, (a, b) => (a + b).ToString());
        result.Should().Be(Result<string, string>.Ok("43"));
    }

    [Test]
    public void ShouldZipValuesIntoTupleReturnErrorWhenFirstResultIsError()
    {
        var r1 = Result.Error<int, string>("Err1");
        var r2 = Result.Ok(1);

        var result = r1.Zip(r2);
        result.Should().Be(Result<(int, int), string>.Error("Err1"));
    }

    [Test]
    public void ShouldZipValuesIntoTupleReturnErrorWhenSecondResultIsError()
    {
        var r1 = Result.Ok(1);
        var r2 = Result.Error<int, string>("Err2");

        var result = r1.Zip(r2);
        result.Should().Be(Result<(int, int), string>.Error("Err2"));
    }

    [Test]
    public void ShouldGetOkValuesFromCollections()
    {
        var results = new Result<int, string>[] { "Err1", 42, "Err2", 99 };

        results.GetOkValues().Should().BeEquivalentTo(
            new[] { 42, 99, }
        );
    }

    [Test]
    public void ShouldGetErrorValuesFromCollections()
    {
        var results = new Result<int, string>[] { "Err1", 42, "Err2", 99 };
        results.GetErrorValues().Should().BeEquivalentTo("Err1", "Err2");
    }

    [Test]
    public void ShouldChooseOnlyOkValuesFromEnumerable()
    {
        var results = new[] { 1, 2, 3, 4 }.ChooseResult(IsEven);

        results.Should().BeEquivalentTo(new[] { 2, 4 });
    }

    [Test]
    public void ShouldTraverseResultIntoSingleErrorResult()
    {
        var results = new Result<int, string>[] { "Err1", 42, "Err2", 99 };
        results.ToSingleResult().Should().Be(
            Result.Error<IReadOnlyList<int>, string>("Err1")
        );
    }

    [Test]
    public void ShouldTraverseResultIntoErrorListResult()
    {
        var results = new Result<int, string>[] { "Err1", 42, "Err2", 99 };
        results.ToSingleResultWithAllErrors().Should()
            .BeOfType<Result<IReadOnlyList<int>, IReadOnlyList<string>>>()
            .And.BeEquivalentTo(new
            {
                IsError = true,
                Value = new[] { "Err1", "Err2" }
            });
    }

    [Test]
    public void ShouldTraverseResultIntoOkListResult()
    {
        var results = new Result<int, string>[] { 42, 99 };
        results.ToSingleResult().Should()
            .BeOfType<Result<IReadOnlyList<int>, string>>()
            .And
            .BeEquivalentTo(new
            {
                IsOk = true,
                Value = new[] { 42, 99 }
            });
    }

    [Test]
    public void ShouldTraverseEnumerableIntoSingleErrorResult()
    {
        var results = new[] { 2, 3, 4, 5 }.ToSingleResult(IsEven);
        results.Should().Be(
            Result.Error<IReadOnlyList<int>, string>("Invalid 3")
        );
    }

    [Test]
    public void ShouldEnumerableResultIntoErrorListResult()
    {
        var results = new[] { 2, 3, 4, 5 }.ToSingleResultWithAllErrors(IsEven);
        results.Should()
            .BeOfType<Result<IReadOnlyList<int>, IReadOnlyList<string>>>()
            .And.BeEquivalentTo(new
            {
                IsError = true,
                Value = new[] { "Invalid 3", "Invalid 5" },
            });
    }

    [Test]
    public void ShouldEnumerableResultIntoOkListResult()
    {
        var results = new[] { 2, 4, 6 }.ToSingleResult(IsEven);
        results.Should()
            .BeOfType<Result<IReadOnlyList<int>, string>>()
            .And
            .BeEquivalentTo(new
            {
                IsOk = true,
                Value = new[] { 2, 4, 6 },
            });
    }
}
