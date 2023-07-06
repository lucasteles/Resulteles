using System.Diagnostics.CodeAnalysis;

namespace Resulteles.Tests;

[SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed")]
[SuppressMessage("ReSharper", "VariableHidesOuterVariable")]
public class Samples
{
    public static void Teste()
    {
        // explicitly
        var resultSuccess = Result.Ok<int, string>(42);
        var resultError = Result.Error<int, string>("Something wrong!");

        var processResult = Result.Try(() => { return 1; });
        var processAsyncResult = Result.TryAsync(async () => { return await Task.FromResult(2); });
    }

    public Result<int, string> GetEvenNumber(int number)
    {
        if (number % 2 is 0)
            return number;
        else
            return $"{number} is not even!";
    }

    public void ReadingResult()
    {
        Result<int, string> result = GetEvenNumber(2);

        // return -1 on error
        int value1 = result.Match(
            n => n,
            error => -1
        );

        result.Switch(
            n => Console.WriteLine($"the number {n} is even"),
            error => Console.WriteLine(error)
        );

        // return 0 on error
        var value2 = result.DefaultValue(0);

        // lazily define the bad case value
        var value3 = result.DefaultWith(e => e.Length);

        // get the value or throw an exeption
        var value4 = result.GetValueOrThrow();

        // try/out pattern
        if (result.TryGet(out var ok, out var error))
            Console.WriteLine($"the number {ok} is even");
        else
            Console.WriteLine(error);
    }

    public void Operators()
    {
        var result1 = Result.Ok<int, string>(1);

        // Map the result value only when it is Ok
        var result2 = result1.Select(n => n + 1);

        // map for ok or error case
        Result<string, int> result3 = result1.Select(n => n.ToString(), err => err.Length);

        // SelectMany / Bind / FlatMamap
        // Nested results transformation
        Result<int, string> resultCombined = result1.SelectMany(n => GetEvenNumber(n));

        // Zip Results together into a tuple
        // only resolves as Ok when both are Ok
        Result<(int, int), string> resultZip = result1.Zip(result2);

        // Zip Results together
        // only resolves as Ok when both are Ok
        Result<int, string> resultSum = result1.Zip(result2, (a, b) => a + b);

        // Enumerate result, empty when error, singleton when error (see ToArray() also)
        IEnumerable<int> resultEnumerable = result1.AsEnumerable();

        // LINQ QUERY
        // you can use linq query to easily express map and bind

        // simple map
        var result2Linq = from r in result1 select r + 1;

        // combining multiple results (fail fast at first non ok value)
        var resultEvenSum =
            from a in GetEvenNumber(2)
            from b in GetEvenNumber(4)
            from c in GetEvenNumber(6)
            select a + b + c;
    }


    public async Task ResultAsync()
    {
        var result = GetEvenNumber(2);

        // async match overload
        int value1 = await result.Match(
            async n =>
            {
                await Task.Delay(n);
                return n;
            },
            _ => -1
        );

        await result.SwitchAsync(
            async n => await Console.Out.WriteLineAsync($"the number {n} is even"),
            async error => await Console.Error.WriteLineAsync(error)
        );

        // async projection counterparts
        Result<int, string> resultWithDelay = await result.SelectAsync(async n =>
        {
            await Task.Delay(n);
            return n;
        });
    }

    public async Task HelpersAsync()
    {
        var results = new[] { GetEvenNumber(2), GetEvenNumber(3), GetEvenNumber(4) };

        IEnumerable<int> okResultsOnly = results.GetOkValues(); // [2,4]

        var numbers = new[] { 1, 2, 3, 4 };
        var calculatedResults = numbers.ChooseResult(x => GetEvenNumber(x)); // [2,4]

        // group a collection of result into a Result of collection
        // collapse to error if any error is in the collection
        Result<IReadOnlyList<int>, string> singleResult = results.GroupResults();

        // Transform Result<Task<int>, string> in a awaitable Task<Result<int, string>>
        Result<int, string> resultWithDelay2 = await results[0].Select(async n =>
        {
            await Task.Delay(n);
            return n;
        }).ToTask();
    }
}
