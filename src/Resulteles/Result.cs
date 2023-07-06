using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Resulteles.Json;

namespace Resulteles;

/// <summary>
/// Represents an successful operation
/// </summary>
public readonly record struct Success;

/// <summary>
/// Helper type for errorValue handling without exceptions.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
[Serializable]
[System.Text.Json.Serialization.JsonConverter(typeof(ResultJsonConverterFactory))]
public readonly struct Result<TOk, TError> : IEquatable<Result<TOk, TError>>
{
    internal TOk? OkValue { get; }
    internal TError? ErrorValue { get; }

    /// <summary>
    /// Returns true if the result is Ok.
    /// </summary>
    [MemberNotNullWhen(true, nameof(OkValue))]
    [MemberNotNullWhen(false, nameof(ErrorValue))]
    public bool IsOk { get; }

    /// <summary>
    /// Returns true if the result is Error.
    /// </summary>
    [MemberNotNullWhen(false, nameof(OkValue))]
    [MemberNotNullWhen(true, nameof(ErrorValue))]
    public bool IsError => !IsOk;

    /// <summary>
    /// The result value
    /// </summary>
    public object Value => IsOk ? OkValue : ErrorValue;

    /// <summary>
    /// Represents an OK or a Successful result. The code succeeded with a value of 'T
    /// </summary>
    public Result(TOk okValue)
    {
        IsOk = true;
        OkValue = okValue;
        ErrorValue = default;
    }

    /// <summary>
    /// Represents an Error or a Failure. The code failed with a value of 'TError representing what went wrong.
    /// </summary>
    public Result(TError error)
    {
        IsOk = false;
        OkValue = default;
        ErrorValue = error;
    }

    /// <summary>
    /// Represents an OK or a Successful result. The code succeeded with a value of 'T
    /// </summary>
    public static Result<TOk, TError> Ok(TOk result) => new(result);

    /// <summary>
    /// Represents an Error or a Failure. The code failed with a value of 'TError representing what went wrong.
    /// </summary>
    public static Result<TOk, TError> Error(TError error) => new(error);

    /// <summary>
    /// Casts an Ok value to Result
    /// </summary>
    public static implicit operator Result<TOk, TError>(TOk value) => new(value);

    /// <summary>
    /// Unsafely casts a Result to Ok value
    /// </summary>
    public static explicit operator TOk(Result<TOk, TError> value) =>
        value.IsOk
            ? value.OkValue
            : throw new ResultInvalidCastException(
                $"Unable to cast 'Error' result value {value.ErrorValue} of type {typeof(TError).FullName} to type {typeof(TOk).FullName}");

    /// <summary>
    /// Unsafely casts a Result to Error value
    /// </summary>
    public static explicit operator TError(Result<TOk, TError> value) =>
        value.IsError
            ? value.ErrorValue
            : throw new ResultInvalidCastException(
                $"Unable to cast 'Ok' result value {value.OkValue} of type {typeof(TOk).FullName} to type {typeof(TError).FullName}");

    /// <summary>
    /// Casts an Error value to Result
    /// </summary>
    public static implicit operator Result<TOk, TError>(TError value) => new(value);

    /// <summary>
    /// Compare Results
    /// </summary>
    public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) =>
        left.Equals(right);

    /// <summary>
    /// Compare Results
    /// </summary>
    public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) =>
        !(left == right);

    /// <inheritdoc />
    public bool Equals(Result<TOk, TError> other) =>
        IsOk == other.IsOk &&
        EqualityComparer<TOk?>.Default.Equals(OkValue, other.OkValue) &&
        EqualityComparer<TError?>.Default.Equals(ErrorValue, other.ErrorValue);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Result<TOk, TError> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(OkValue, ErrorValue, IsOk);

    string DebuggerDisplay() => IsOk ? $"Ok({OkValue})" : $"Error({ErrorValue})";

    /// <inheritdoc />
    public override string? ToString() => IsOk ? OkValue.ToString() : ErrorValue.ToString();

    /// <summary>
    /// Match the result to obtain the value
    /// </summary>
    public T Match<T>(Func<TOk, T> ok, Func<TError, T> error) =>
        IsOk ? ok(OkValue) : error(ErrorValue);

    /// <summary>
    /// Switch the result to process value
    /// </summary>
    public void Switch(Action<TOk> ok, Action<TError> error)
    {
        if (IsOk)
            ok(OkValue);
        else
            error(ErrorValue);
    }
}
