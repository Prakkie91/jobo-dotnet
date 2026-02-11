namespace Jobo.Enterprise.Client.Exceptions;

/// <summary>
/// Base exception for all Jobo client errors.
/// </summary>
public class JoboException : Exception
{
    public int? StatusCode { get; }
    public string? Detail { get; }
    public string? ResponseBody { get; }

    public JoboException(string message, int? statusCode = null, string? detail = null, string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        Detail = detail;
        ResponseBody = responseBody;
    }
}

/// <summary>
/// Raised when the API key is missing or invalid (401).
/// </summary>
public class JoboAuthenticationException : JoboException
{
    public JoboAuthenticationException(string message, int? statusCode = null, string? detail = null, string? responseBody = null)
        : base(message, statusCode, detail, responseBody) { }
}

/// <summary>
/// Raised when the rate limit is exceeded (429).
/// </summary>
public class JoboRateLimitException : JoboException
{
    public int? RetryAfterSeconds { get; }

    public JoboRateLimitException(string message, int? retryAfterSeconds = null, int? statusCode = null, string? detail = null, string? responseBody = null)
        : base(message, statusCode, detail, responseBody)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}

/// <summary>
/// Raised when the request is invalid (400).
/// </summary>
public class JoboValidationException : JoboException
{
    public JoboValidationException(string message, int? statusCode = null, string? detail = null, string? responseBody = null)
        : base(message, statusCode, detail, responseBody) { }
}

/// <summary>
/// Raised when the server returns a 5xx error.
/// </summary>
public class JoboServerException : JoboException
{
    public JoboServerException(string message, int? statusCode = null, string? detail = null, string? responseBody = null)
        : base(message, statusCode, detail, responseBody) { }
}
