using System.Text.Json.Serialization;

namespace GemiNet;

public record Model
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("baseModelId")]
    public string? BaseModelId { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("inputTokenLimit")]
    public int? InputTokenLimit { get; init; }

    [JsonPropertyName("outputTokenLimit")]
    public int? OutputTokenLimit { get; init; }

    [JsonPropertyName("supportedGenerationMethods")]
    public string[]? SupportedGenerationMethods { get; init; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    [JsonPropertyName("maxTemperature")]
    public double? MaxTemperature { get; init; }

    [JsonPropertyName("topP")]
    public double? TopP { get; init; }

    [JsonPropertyName("topK")]
    public int? TopK { get; init; }
}

public record ListModelsRequest
{
    public int? PageSize { get; init; }
    public string? PageToken { get; init; }
}

public record ListModelsResponse
{
    [JsonPropertyName("models")]
    public required Model[] Models { get; init; }

    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; init; }
}
