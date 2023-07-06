using System.Threading.Tasks;

namespace Resulteles;

/// <summary>
/// Helper type for errorValue handling without exceptions.
/// </summary>
public static class Result
{
    /// <summary>
    /// Represents an OK or a Successful result. The code succeeded with a value of 'T
    /// </summary>
    public static Result<TOk, TError> Ok<TOk, TError>(TOk result) =>
        Result<TOk, TError>.Ok(result);

    /// <summary>
    /// Represents an OK or a Successful result. The code succeeded with a value of 'T
    /// </summary>
    public static Result<TOk, string> Ok<TOk>(TOk result) =>
        Result<TOk, string>.Ok(result);

    /// <summary>
    /// Represents an Error or a Failure. The code failed with a value of 'TError representing what went wrong.
    /// </summary>
    public static Result<TOk, TError> Error<TOk, TError>(TError error) =>
        Result<TOk, TError>.Error(error);

    /// <summary>
    /// Represents an Error or a Failure. The code failed with a value of 'TError representing what went wrong.
    /// </summary>
    public static Result<Success, TError> Error<TError>(TError error) =>
        Result<Success, TError>.Error(error);

    /// <summary>
    /// Try run function, catching exceptions as a result error value
    /// </summary>
    public static Result<TOk, Exception> Try<TOk>(Func<TOk> func)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Try run function, catching exceptions as a result error value
    /// </summary>
    public static async Task<Result<TOk, Exception>> TryAsync<TOk>(Func<Task<TOk>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
