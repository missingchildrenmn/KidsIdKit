namespace KidsIdKit.Core.Data;

/// <summary>
/// Exception thrown when data access operations fail.
/// </summary>
public class DataAccessException : Exception
{
    public DataAccessErrorType ErrorType { get; }

    public DataAccessException(DataAccessErrorType errorType, string message)
        : base(message)
    {
        ErrorType = errorType;
    }

    public DataAccessException(DataAccessErrorType errorType, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorType = errorType;
    }
}

/// <summary>
/// Types of data access errors for better error handling.
/// </summary>
public enum DataAccessErrorType
{
    /// <summary>Data could not be read from storage.</summary>
    ReadFailure,

    /// <summary>Data could not be written to storage.</summary>
    WriteFailure,

    /// <summary>Data was corrupted or could not be deserialized.</summary>
    CorruptedData,

    /// <summary>Encryption or decryption failed.</summary>
    EncryptionFailure,

    /// <summary>Storage is full or quota exceeded.</summary>
    StorageFull,

    /// <summary>Unknown or unexpected error.</summary>
    Unknown
}
