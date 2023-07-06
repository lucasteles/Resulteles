using System.Threading.Tasks;

namespace Resulteles;

/// <summary>
/// Result TPL Extensions
/// </summary>
public static class ResultAsyncExtensions
{
    /// <summary>
    /// Convert result of task into task of result
    /// </summary>
    public static async Task<Result<TOk, TError>> ToTask<TOk, TError>(
        this Result<Task<TOk>, TError> result)
    {
        if (result.IsOk) return await result.OkValue;
        return result.ErrorValue;
    }

    /// <summary>
    /// Convert result of task into task of result
    /// </summary>
    public static async Task<Result<TOk, TError>> ToTask<TOk, TError>(
        this Result<TOk, Task<TError>> result)
    {
        if (result.IsOk) return result.OkValue;
        return await result.ErrorValue;
    }

    /// <summary>
    /// Convert result of task into task of result
    /// </summary>
    public static async Task<Result<TOk, TError>> ToTask<TOk, TError>(
        this Result<Task<TOk>, Task<TError>> result)
    {
        if (result.IsOk) return await result.OkValue;
        return await result.ErrorValue;
    }

    /// <summary>
    /// Run side effect when result is OK
    /// </summary>
    public static async Task<Result<TOk, TError>> Tap<TOk, TError>(
        this Result<TOk, TError> result, Func<TOk, Task> action)
    {
        if (result.IsOk) await action(result.OkValue);
        return result;
    }

    /// <summary>
    /// Match the result to obtain the value
    /// </summary>
    public static async Task<T> Match<TOk, TError, T>(this Result<TOk, TError> result,
        Func<TOk, Task<T>> ok,
        Func<TError, T> error) =>
        result.IsOk ? await ok(result.OkValue) : error(result.ErrorValue);

    /// <summary>
    /// Match the result to obtain the value
    /// </summary>
    public static async Task<T>
        Match<TOk, TError, T>(this Result<TOk, TError> result,
            Func<TOk, T> ok,
            Func<TError, Task<T>> error) =>
        result.IsOk ? ok(result.OkValue) : await error(result.ErrorValue);

    /// <summary>
    /// Switch the result to process value
    /// </summary>
    public static async Task SwitchAsync<TOk, TError>(
        this Result<TOk, TError> result,
        Func<TOk, Task> ok,
        Func<TError, Task> error)
    {
        if (result.IsOk)
            await ok(result.OkValue);
        else
            await error(result.ErrorValue);
    }

    /// <summary>
    /// Switch the result to process value
    /// </summary>
    public static async Task SwitchAsync<TOk, TError>(this Result<TOk, TError> result,
        Func<TOk, Task> ok, Action<TError> error)
    {
        if (result.IsOk)
            await ok(result.OkValue);
        else
            error(result.ErrorValue);
    }

    /// <summary>
    /// Switch the result to process value
    /// </summary>
    public static async Task SwitchAsync<TOk, TError>(this Result<TOk, TError> result,
        Action<TOk> ok,
        Func<TError, Task> error)
    {
        if (result.IsOk)
            ok(result.OkValue);
        else
            await error(result.ErrorValue);
    }

    /// <summary>
    /// Projects ok result value into a new form.
    /// </summary>e
    public static Task<Result<TMap, TError>> SelectAsync<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, Task<TMap>> selector
    ) => result.Select(selector).ToTask();

    /// <summary>
    /// Projects ok result value into a new form.
    /// </summary>e
    public static Task<Result<TMapOk, TMapError>> SelectAsync<TOk, TError, TMapOk, TMapError>(
        this Result<TOk, TError> result,
        Func<TOk, Task<TMapOk>> okSelector,
        Func<TError, Task<TMapError>> errorSelector
    ) => result.Select(okSelector, errorSelector).ToTask();

    /// <summary>
    /// Projects ok result value into a new form.
    /// </summary>e
    public static Task<Result<TMapOk, TMapError>> SelectAsync<TOk, TError, TMapOk, TMapError>(
        this Result<TOk, TError> result,
        Func<TOk, TMapOk> okSelector,
        Func<TError, Task<TMapError>> errorSelector
    ) => result.Select(okSelector, errorSelector).ToTask();

    /// <summary>
    /// Projects ok result value into a new form.
    /// </summary>e
    public static Task<Result<TMapOk, TMapError>> SelectAsync<TOk, TError, TMapOk, TMapError>(
        this Result<TOk, TError> result,
        Func<TOk, Task<TMapOk>> okSelector,
        Func<TError, TMapError> errorSelector
    ) => result.Select(okSelector, errorSelector).ToTask();

    /// <summary>
    /// Projects ok result value into a new form.
    /// </summary>e
    public static Task<Result<TOk, TMap>> SelectErrorAsync<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TError, Task<TMap>> selector
    ) => result.SelectError(selector).ToTask();


    /// <summary>
    /// Projects ok result value into a new flattened form.
    /// </summary>
    public static Task<Result<TMap, TError>> SelectManyAsync<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, Result<Task<TMap>, TError>> bind) =>
        result.SelectMany(bind).ToTask();

    /// <summary>
    /// Projects ok result value into a new flattened form.
    /// </summary>
    public static Task<Result<TMap, TError>> SelectManyAsync<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, Result<TMap, Task<TError>>> bind) =>
        result.SelectError(Task.FromResult).SelectMany(bind).ToTask();

    /// <summary>
    /// Projects ok result value into a new flattened form.
    /// </summary>
    public static Task<Result<TMap, TError>> SelectManyAsync<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, Result<Task<TMap>, Task<TError>>> bind) =>
        result.SelectError(Task.FromResult).SelectMany(bind).ToTask();

    /// <summary>
    /// Projects ok result value into a new flattened form.
    /// </summary>
    public static async Task<Result<TMap, TError>> SelectManyAsync<TOk, TError, TMap>(
        this Result<TOk, TError> result,
        Func<TOk, Task<Result<TMap, TError>>> bind) =>
        result.IsError ? new(result.ErrorValue) : await bind(result.OkValue);
}
