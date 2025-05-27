using System.Text.Json.Serialization;

namespace GemiNet;

public record CountTokensRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("contents")]
    public required Contents Contents { get; init; }

    [JsonPropertyName("generateContentRequest")]
    public GenerateContentRequest? GenerateContentRequest { get; init; }
}

public record CountTokensResponse
{
    [JsonPropertyName("totalTokens")]
    public double? TotalTokens { get; init; }

    [JsonPropertyName("cachedContentTokenCount")]
    public double? CachedContentTokenCount { get; init; }
}