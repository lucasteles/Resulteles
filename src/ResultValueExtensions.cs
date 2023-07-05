using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Resulteles;

/// <summary>
/// Result value extensions
/// </summary>
public static class ResultValueExtensions
{
    /// <summary>
    /// Attempts to extract value from container if it is present.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="value">Extracted value.</param>
    /// <returns><see langword="true"/> if value is present; otherwise, <see langword="false"/>.</returns>
    public static bool TryOk<TOk, TError>(
        this Result<TOk, TError> result,
        [NotNullWhen(true)] out TOk? value)
    {
        value = result.OkValue;
        return result.IsOk;
    }

    /// <summary>
    /// Attempts to extract error from container if it is present.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="value">Extracted value.</param>
    /// <returns><see langword="true"/> if value is present; otherwise, <see langword="false"/>.</returns>
    public static bool TryError<TOk, TError>(
        this Result<TOk, TError> result,
        [NotNullWhen(true)] out TError? value)
    {
        value = result.ErrorValue;
        return result.IsError;
    }

    /// <summary>
    /// Attempts to extract value from container if it is present.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="value">Extracted value.</param>
    /// <param name="error">Extracted error.</param>
    /// <returns><see langword="true"/> if value is present; otherwise, <see langword="false"/>.</returns>
    public static bool TryGet<TOk, TError>(
        this Result<TOk, TError> result,
        [NotNullWhen(true)] out TOk? value,
        [NotNullWhen(false)] out TError? error
    )
    {
        value = result.OkValue;
        error = result.ErrorValue;
        return result.IsOk;
    }

    /// <summary>
    /// Get value if result is Ok otherwise throws
    /// </summary>
    public static TOk GetValueOrThrow<TOk, TError>(this Result<TOk, TError> result)
    {
        if (result.IsError)
            if (result.ErrorValue is Exception exception)
                throw exception;
            else
                throw new ResultInvalidException($"{result.ErrorValue}");

        return result.OkValue;
    }

    /// <summary>
    /// Get value if result is Ok otherwise throws
    /// </summary>
    public static TOk GetValueOrThrow<TOk, TError>(
        this Result<TOk, TError> result,
        Func<TError, string> formatMessage)
    {
        if (result.IsError)
            throw new ResultInvalidException(formatMessage(result.ErrorValue));

        return result.OkValue;
    }

    /// <summary>
    /// Get value if result is Ok otherwise throws
    /// </summary>
    public static TOk GetValueOrThrow<TOk, TError>(
        this Result<TOk, TError> result,
        Func<TError, Exception> getException)
    {
        if (result.IsError)
            throw getException(result.ErrorValue);

        return result.OkValue;
    }

    /// <summary>
    /// Throws on error result
    /// </summary>
    public static void ThrowIfError<TOk, TError>(this Result<TOk, TError> result)

    {
        if (!result.IsError) return;

        if (result.ErrorValue is Exception exception)
            throw exception;

        throw new ResultInvalidException($"{result.ErrorValue}");
    }

    /// <summary>
    /// Get value if result is Ok otherwise throws
    /// </summary>
    public static void ThrowIfError<TOk, TError>(this Result<TOk, TError> result,
        Func<TError, string> formatMessage)
    {
        if (result.IsError)
            throw new ResultInvalidException(formatMessage(result.ErrorValue));
    }

    /// <summary>
    /// Get value if result is Ok otherwise throws
    /// </summary>
    public static void ThrowIfError<TOk, TError>(this Result<TOk, TError> result,
        Func<TError, Exception> getException)
    {
        if (result.IsError)
            throw getException(result.ErrorValue);
    }

    /// <summary>
    /// Gets the value of the result if the result is Ok, otherwise returns the specified default value.
    /// </summary>
    public static TOk DefaultValue<TOk, TError>(this Result<TOk, TError> result, TOk value) =>
        result.Match(ok => ok, _ => value);

    /// <summary>
    /// Gets the value of the result if the result is Ok, otherwise evaluates defThunk and returns the result
    /// </summary>
    public static TOk DefaultWith<TOk, TError>(this Result<TOk, TError> result,
        Func<TError, TOk> defThunk) => result.Match(ok => ok, defThunk);

    /// <summary>
    /// Deconstructs result value into (IsOk, OkValue?, ErrorValue?)
    /// </summary>
    public static void Deconstruct<TOk, TError>(
        this Result<TOk, TError> result,
        out bool success,
        out TOk? ok,
        out TError? error)
        where TOk : struct
        where TError : struct
        => (success, ok, error) = (result.IsOk, result.OkValue, result.ErrorValue);

    /// <summary>
    /// Deconstructs result value into (IsOk, OkValue?, ErrorValue?)
    /// </summary>
    public static void Deconstruct<TOk, TError>(
        this Result<TOk, TError> result,
        out bool success,
        out TOk? ok,
        out TError? error)
        where TOk : class
        where TError : class
        => (success, ok, error) = (result.IsOk, result.OkValue, result.ErrorValue);

    /// <summary>
    /// Deconstructs result value into (IsOk, OkValue?, ErrorValue?)
    /// </summary>
    public static void Deconstruct<TOk, TError>(
        this Result<TOk, TError> result,
        out bool success,
        out TOk? ok,
        out TError? error)
        where TOk : struct
        where TError : class
        => (success, ok, error) = (result.IsOk, result.OkValue, result.ErrorValue);

    /// <summary>
    /// Deconstructs result value into (IsOk, OkValue?, ErrorValue?)
    /// </summary>
    public static void Deconstruct<TOk, TError>(
        this Result<TOk, TError> result,
        out bool success,
        out TOk? ok,
        out TError? error)
        where TOk : class
        where TError : struct
        => (success, ok, error) = (result.IsOk, result.OkValue, result.ErrorValue);

    /// <summary>
    /// Convert value type result to nullable
    /// </summary>
    public static Result<TOk?, TError> AsNullable<TOk, TError>(
        this Result<TOk, TError> result)
        where TOk : struct =>
        result.Select(x => (TOk?)x);

    /// <summary>
    /// Convert result of task into task of result
    /// </summary>
    public static Result<IReadOnlyList<TOk>, TError> ToResult<TOk, TError>(
        this IEnumerable<Result<TOk, TError>> results)
    {
        List<TOk> okResults = new();
        foreach (var result in results)
        {
            if (result.IsOk)
                okResults.Add(result.OkValue);
            else
                return new(result.ErrorValue);
        }

        return new(new ReadOnlyCollection<TOk>(okResults));
    }

    /// <summary>
    /// If a result is successful, returns it, otherwise <see langword="null"/>.
    /// </summary>
    /// <returns>Nullable value.</returns>
    public static T? ToNullable<T, TError>(this Result<T, TError> valueResult)
        where T : struct
        =>
            valueResult.IsOk ? valueResult.OkValue : null;
}
