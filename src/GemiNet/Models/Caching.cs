using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GemiNet;

public record CreateChachedContentRequest
{
    public required string Model { get; init; }
    public Contents? Contents { get; init; }
    public Tool[]? Tools { get; init; }
    public string? ExpireTime { get; init; }
    public string? Ttl { get; init; }
    public string? DisplayName { get; init; }
    public Content? SystemInstruction { get; init; }
    public ToolConfig? ToolConfig { get; init; }
}

public record ListCachedContentsRequest
{
    public int? PageSize { get; init; }
    public string? PageToken { get; init; }
}

public record ListCachedContentsResponse
{
    [JsonPropertyName("cachedContents")]
    public CachedContent[]? CachedContents { get; init; }

    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; init; }
}

public record GetChachedContentRequest
{
    public required string Name { get; init; }
}

public record PatchChachedContentRequest
{
    public required string Name { get; init; }
    public string? UpdateMask { get; init; }
    public PatchChachedContentConfig? Config { get; init; }
}

public record DeleteChachedContentRequest
{
    public required string Name { get; init; }
}

public record PatchChachedContentConfig
{
    [JsonPropertyName("expireTime")]
    public string? ExpireTime { get; init; }

    [JsonPropertyName("ttl")]
    public string? Ttl { get; init; }
}

public record CachedContent
{
    [JsonPropertyName("contents")]
    public Contents? Contents { get; init; }

    [JsonPropertyName("tools")]
    public Tool[]? Tools { get; init; }

    [JsonPropertyName("createTime")]
    public string? CreateTime { get; init; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; init; }

    [JsonPropertyName("usageMetadata")]
    public CachingUsageMetadata? UsageMetadata { get; init; }

    [JsonPropertyName("expireTime")]
    public string? ExpireTime { get; init; }

    [JsonPropertyName("ttl")]
    public string? Ttl { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("systemInstruction")]
    public Content? SystemInstruction { get; init; }

    [JsonPropertyName("toolConfig")]
    public ToolConfig? ToolConfig { get; init; }
}

public record Content
{
    [JsonPropertyName("parts")]
    public required Part[] Parts { get; init; }

    [JsonPropertyName("role")]
    public string? Role { get; init; }

    public static implicit operator Content(string text)
    {
        return new()
        {
            Parts = [new() { Text = text }]
        };
    }
}

public class Contents : Collection<Content>
{
    public static implicit operator Contents(string text)
    {
        return [(Content)text];
    }

    public static implicit operator Contents(Content content)
    {
        return [content];
    }
}

public record Part
{
    [JsonPropertyName("thought")]
    public bool? Thought { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("inlineData")]
    public Blob? InlineData { get; init; }

    [JsonPropertyName("functionCall")]
    public FunctionCall? FunctionCall { get; init; }

    [JsonPropertyName("functionResponse")]
    public FunctionResponse? FunctionResponse { get; init; }

    [JsonPropertyName("fileData")]
    public FileData? FileData { get; init; }

    [JsonPropertyName("executableCode")]
    public ExecutableCode? ExecutableCode { get; init; }

    [JsonPropertyName("codeExecutionResult")]
    public CodeExecutionResult? CodeExecutionResult { get; init; }
}

public record Blob
{
    [JsonPropertyName("mimeType")]
    public required string MimeType { get; init; }

    [JsonPropertyName("data")]
    public required string Data { get; init; }
}

public record FunctionCall
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("args")]
    public JsonElement? Args { get; init; }
}

public record FunctionResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("response")]
    public JsonElement? Response { get; init; }
}

public record FileData
{
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; init; }

    [JsonPropertyName("fileUri")]
    public required string FileUri { get; init; }
}

public record ExecutableCode
{
    [JsonPropertyName("language")]
    public required ExecutableCodeLanguage Language { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }
}

public enum ExecutableCodeLanguage
{
    [JsonStringEnumMemberName("LANGUAGE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("PYTHON")]
    Python,
}

public record CodeExecutionResult
{
    [JsonPropertyName("outcome")]
    public required CodeExecutionOutcome Outcome { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}

public enum CodeExecutionOutcome
{
    [JsonStringEnumMemberName("OUTCOME_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("OUTCOME_OK")]
    Ok,

    [JsonStringEnumMemberName("OUTCOME_FAILED")]
    Failed,

    [JsonStringEnumMemberName("OUTCOME_DEADLINE_EXCEEDED")]
    DeadlineExceeded,
}

public record Tool
{
    [JsonPropertyName("functionDeclarations")]
    public FunctionDeclaration[]? FunctionDeclarations { get; init; }

    [JsonPropertyName("googleSearchRetrieval")]
    public GoogleSearchRetrieval? GoogleSearchRetrieval { get; init; }

    [JsonPropertyName("codeExecution")]
    public CodeExecution? CodeExecution { get; init; }

    [JsonPropertyName("googleSearch")]
    public GoogleSearch? GoogleSearch { get; init; }
}

public record FunctionDeclaration
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("parameters")]
    public Schema? Parameters { get; init; }

    [JsonPropertyName("response")]
    public Schema? Response { get; init; }
}

public record Schema
{
    [JsonPropertyName("type")]
    public required DataType Type { get; init; }

    [JsonPropertyName("format")]
    public string? Format { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("nullable")]
    public bool? Nullable { get; init; }

    [JsonPropertyName("enum")]
    public string[]? Enum { get; init; }

    [JsonPropertyName("maxItems")]
    public string? MaxItems { get; init; }

    [JsonPropertyName("minItems")]
    public string? MinItems { get; init; }

    [JsonPropertyName("properties")]
    public Dictionary<string, Schema>? Properties { get; init; }

    [JsonPropertyName("required")]
    public string[]? Required { get; init; }

    [JsonPropertyName("minProperties")]
    public string? MinProperties { get; init; }

    [JsonPropertyName("maxProperties")]
    public string? MaxProperties { get; init; }

    [JsonPropertyName("minLength")]
    public string? MinLength { get; init; }

    [JsonPropertyName("maxLength")]
    public string? MaxLength { get; init; }

    [JsonPropertyName("pattern")]
    public string? Pattern { get; init; }

    [JsonPropertyName("example")]
    public string? Example { get; init; }

    [JsonPropertyName("anyOf")]
    public Schema[]? AnyOf { get; init; }

    [JsonPropertyName("propertyOrdering")]
    public string[]? PropertyOrdering { get; init; }

    [JsonPropertyName("default")]
    public JsonElement? Default { get; init; }

    [JsonPropertyName("items")]
    public Schema[]? Items { get; init; }

    [JsonPropertyName("minimum")]
    public double? Minimum { get; init; }

    [JsonPropertyName("maximum")]
    public double? Maximum { get; init; }

    public static Schema FromJsonElement(JsonElement element)
    {
        return JsonSerializer.Deserialize(element, GemiNetJsonSerializerContext.Default.GetTypeInfo<Schema>()!)
            ?? new() { Type = DataType.Unspecified };
    }
}


public enum DataType
{
    [JsonStringEnumMemberName("TYPE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("STRING")]
    String,

    [JsonStringEnumMemberName("NUMBER")]
    Number,

    [JsonStringEnumMemberName("INTEGER")]
    Integer,

    [JsonStringEnumMemberName("BOOLEAN")]
    Boolean,

    [JsonStringEnumMemberName("ARRAY")]
    Array,

    [JsonStringEnumMemberName("OBJECT")]
    Object,

    [JsonStringEnumMemberName("NULL")]
    Null,
}


public record GoogleSearchRetrieval
{
    [JsonPropertyName("dynamicRetrievalConfig")]
    public required DynamicRetrievalConfig DynamicRetrievalConfig { get; init; }
}

public record DynamicRetrievalConfig
{
    [JsonPropertyName("mode")]
    public required DynamicRetrievalMode Mode { get; init; }

    [JsonPropertyName("dynamicThreshold")]
    public required double DynamicThreshold { get; init; }
}

public enum DynamicRetrievalMode
{
    [JsonStringEnumMemberName("MODE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("MODE_DYNAMIC")]
    Dynamic,
}

public record CodeExecution;
public record GoogleSearch;


public record ToolConfig
{
    [JsonPropertyName("functionCallingConfig")]
    public required FunctionCallingConfig FunctionCallingConfig { get; init; }
}

public record FunctionCallingConfig
{
    [JsonPropertyName("mode")]
    public required FunctionCallingMode Mode { get; init; }

    [JsonPropertyName("allowedFunctionNames")]
    public required string[] AllowedFunctionNames { get; init; }
}

public enum FunctionCallingMode
{
    [JsonStringEnumMemberName("MODE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("AUTO")]
    Auto,

    [JsonStringEnumMemberName("ANY")]
    Any,

    [JsonStringEnumMemberName("NONE")]
    None,

    [JsonStringEnumMemberName("VALIDATED")]
    Validated,
}

public record CachingUsageMetadata
{
    [JsonPropertyName("totalTokenCount")]
    public required int TotalTokenCount { get; init; }
}