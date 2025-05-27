using System.Text.Json.Serialization;

namespace GemiNet;

public class GemiNetException : Exception
{
    public GemiNetException(string? message) : base(message)
    {
    }

    public GemiNetException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}


public record ErrorResponse
{
    [JsonPropertyName("error")]
    public required ErrorResponseData Error { get; init; }
}

public record ErrorResponseData
{
    [JsonPropertyName("code")]
    public required int Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }
}