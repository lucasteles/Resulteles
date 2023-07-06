namespace Resulteles;

/// <summary>
/// Result assertion extensions
/// </summary>
public static class ResultFluentAssertionsExtensions
{
    /// <summary>
    /// Simple result assertions
    /// </summary>
    public static ResultSimpleAssertions Should(this Result<Success, string> result) => new(result);

    /// <summary>
    /// String Error result assertions
    /// </summary>
    public static ResultTStringAssertions<T> Should<T>(this Result<T, string> result) => new(result);

    /// <summary>
    /// Result assertions
    /// </summary>
    public static ResultTypeAssertions<TOk, TError> Should<TOk, TError>(this Result<TOk, TError> result) => new(result);
}
