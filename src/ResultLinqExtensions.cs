namespace Resulteles;

/// <summary>
/// Result LINQ extensions
/// </summary>
public static class ResultLinqExtensions
{
    /// <summary>
    /// Convert the result to an enumerable of length 0 or 1.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<TOk> AsEnumerable<TOk, TError>(this Result<TOk, TError> result)
    {
        if (result.IsOk)
            yield return result.OkValue;
    }

    /// <summary>
    /// Convert the result to an array of length 0 or 1.
    /// </summary>
    public static TOk[] ToArray<TOk, TError>(this Result<TOk, TError> result) =>
        result.IsOk ? new[] { result.OkValue } : Array.Empty<TOk>();

    /// <summary>
    /// Projects ok result value into a new form.
    /// </summary>
    public static Result<TMap, TError> Select<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, TMap> selector
    ) => result.Match(
        ok => new Result<TMap, TError>(selector(ok)),
        error => new(error)
    );

    /// <summary>
    /// Projects ok and error result values into a new form.
    /// </summary>
    public static Result<TMapOk, TMapError> Select<TOk, TError, TMapOk, TMapError>(
        this Result<TOk, TError> result,
        Func<TOk, TMapOk> okSelector,
        Func<TError, TMapError> errorSelector
    ) => result.Match(
        ok => new Result<TMapOk, TMapError>(okSelector(ok)),
        error => new(errorSelector(error))
    );

    /// <summary>
    /// Projects error result element into a new form.
    /// </summary>
    public static Result<TOk, TMap> SelectError<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TError, TMap> selector) =>
        result.Match(
            ok => new Result<TOk, TMap>(ok),
            error => new(selector(error))
        );

    /// <summary>
    /// Projects ok result value into a new flattened form.
    /// </summary>
    public static Result<TMap, TError> SelectMany<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, Result<TMap, TError>> bind) =>
        result.IsError ? new(result.ErrorValue) : bind(result.OkValue);

    /// <summary>
    /// Projects ok result value into a new flattened form.
    /// </summary>
    public static Result<TResult, TError> SelectMany<TOk, TError, TMap, TResult>(
        this Result<TOk, TError> result,
        Func<TOk, Result<TMap, TError>> bind,
        Func<TOk, TMap, TResult> project) =>
        result.SelectMany(a => bind(a).Select(b => project(a, b)));

    /// <summary>
    /// Maps a Result value from a pair of Result values.
    /// </summary>
    public static Result<TResult, TError> Zip<TOk, TError, TResult, TOther>(
        this Result<TOk, TError> result,
        Result<TOther, TError> other,
        Func<TOk, TOther, TResult> selector)
    {
        if (result.IsError)
            return new(result.ErrorValue);

        if (other.IsError)
            return new(other.ErrorValue);

        return new(selector(result.OkValue, other.OkValue));
    }

    /// <summary>
    /// Creates a Result value from a pair of Result values.
    /// </summary>
    public static Result<(TOk, TOther), TError> Zip<TOk, TError, TOther>(
        this Result<TOk, TError> result,
        Result<TOther, TError> other) =>
        result.Zip(other, (a, b) => (a, b));

    /// <summary>
    /// Return new collection with ok values only
    /// </summary>
    public static IEnumerable<TOk> Choose<TOk, TError>(
        this IEnumerable<Result<TOk, TError>> results
    ) =>
        from result in results where result.IsOk select result.OkValue;

    /// <summary>
    /// Return new collection with ok values only
    /// </summary>
    public static IEnumerable<TError> ChooseErrors<TOk, TError>(
        this IEnumerable<Result<TOk, TError>> results) =>
        from result in results where result.IsError select result.ErrorValue;
}
