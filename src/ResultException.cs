using System.Runtime.Serialization;

namespace Resulteles;

/// <summary>
/// The exception that is thrown when a invalid explicit cast is made on a result.
/// </summary>
[Serializable]
public class ResultInvalidCastException : Exception
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
public class ResultException : Exception
{
    /// <inheritdoc />
    internal ResultException(string message) : base(message) { }

    /// <inheritdoc />
    protected ResultException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
