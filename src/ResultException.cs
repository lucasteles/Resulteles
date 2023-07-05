using System.Runtime.Serialization;

#pragma warning disable CS0628

namespace Resulteles;

/// <summary>
/// The exception that is thrown when a invalid explicit cast is made on a result.
/// </summary>
[Serializable]
public sealed class ResultInvalidCastException : Exception
{
    /// <inheritdoc />
    internal ResultInvalidCastException(string message) : base(message) { }

    /// <inheritdoc />
    protected ResultInvalidCastException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

/// <summary>
/// The exception that is thrown for invalid result values.
/// </summary>
[Serializable]
public sealed class ResultInvalidException : Exception
{
    /// <inheritdoc />
    internal ResultInvalidException(string message) : base(message) { }

    /// <inheritdoc />
    protected ResultInvalidException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
