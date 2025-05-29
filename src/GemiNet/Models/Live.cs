using System.Text.Json.Serialization;

namespace GemiNet;

public record BidiGenerateContent
{
    [JsonPropertyName("setup")]
    public BidiGenerateContentSetup? Setup { get; init; }

    [JsonPropertyName("clientContent")]
    public BidiGenerateContentClientContent? ClientContent { get; init; }

    [JsonPropertyName("realtimeInput")]
    public BidiGenerateContentRealtimeInput? RealtimeInput { get; init; }

    [JsonPropertyName("toolResponse")]
    public BidiGenerateContentToolResponse? ToolResponse { get; init; }
}

public record ActivityEnd;

public enum ActivityHandling
{
    [JsonStringEnumMemberName("ACTIVITY_HANDLING_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("START_OF_ACTIVITY_INTERRUPTS")]
    StartOfActivityInterrupts,

    [JsonStringEnumMemberName("NO_INTERRUPTION")]
    NoInterruption,
}

public record ActivityStart;

public record AudioTranscriptionConfig;

public record AutomaticActivityDetection
{
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }

    [JsonPropertyName("startOfSpeechSensitivity")]
    public StartSensitivity? StartOfSpeechSensitivity { get; init; }

    [JsonPropertyName("prefixPaddingMs")]
    public int? PrefixPaddingMs { get; init; }

    [JsonPropertyName("endOfSpeechSensitivity")]
    public EndSensitivity? EndOfSpeechSensitivity { get; init; }

    [JsonPropertyName("silenceDurationMs")]
    public int? SilenceDurationMs { get; init; }
}

public record BidiGenerateContentClientContent
{
    [JsonPropertyName("turns")]
    public Contents? Turns { get; init; }

    [JsonPropertyName("turnComplete")]
    public bool? TurnComplete { get; init; }
}

public record BidiGenerateContentRealtimeInput
{
    [JsonPropertyName("mediaChunks")]
    public Blob[]? MediaChunks { get; init; }

    [JsonPropertyName("audio")]
    public Blob? Audio { get; init; }

    [JsonPropertyName("video")]
    public Blob? Video { get; init; }

    [JsonPropertyName("activityStart")]
    public ActivityStart? ActivityStart { get; init; }

    [JsonPropertyName("activityEnd")]
    public ActivityEnd? ActivityEnd { get; init; }

    [JsonPropertyName("audioStreamEnd")]
    public bool? AudioStreamEnd { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public record BidiGenerateContentServerContent
{
    [JsonPropertyName("generationComplete")]
    public bool? GenerationComplete { get; init; }

    [JsonPropertyName("turnComplete")]
    public bool? TurnComplete { get; init; }

    [JsonPropertyName("interrupted")]
    public bool? Interrupted { get; init; }

    [JsonPropertyName("groundingMetadata")]
    public GroundingMetadata? GroundingMetadata { get; init; }

    [JsonPropertyName("inputTranscription")]
    public BidiGenerateContentTranscription? InputTranscription { get; init; }

    [JsonPropertyName("outputTranscription")]
    public BidiGenerateContentTranscription? OutputTranscription { get; init; }

    [JsonPropertyName("urlContextMetadata")]
    public UrlContextMetadata? UrlContextMetadata { get; init; }

    [JsonPropertyName("modelTurn")]
    public Content? ModelTurn { get; init; }
}

public record BidiGenerateContentServerMessage
{
    [JsonPropertyName("usageMetadata")]
    public LiveUsageMetadata? UsageMetadata { get; init; }

    [JsonPropertyName("setupComplete")]
    public BidiGenerateContentSetupComplete? SetupComplete { get; init; }

    [JsonPropertyName("serverContent")]
    public BidiGenerateContentServerContent? ServerContent { get; init; }

    [JsonPropertyName("toolCall")]
    public BidiGenerateContentToolCall? ToolCall { get; init; }

    [JsonPropertyName("toolCallCancellation")]
    public BidiGenerateContentToolCallCancellation? ToolCallCancellation { get; init; }

    [JsonPropertyName("goAway")]
    public GoAway? GoAway { get; init; }

    [JsonPropertyName("sessionResumptionUpdate")]
    public SessionResumptionUpdate? SessionResumptionUpdate { get; init; }
}

public record BidiGenerateContentSetup
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("generationConfig")]
    public GenerationConfig? Config { get; init; }

    [JsonPropertyName("systemInstruction")]
    public Content? SystemInstruction { get; init; }

    [JsonPropertyName("tools")]
    public Tool[]? Tools { get; init; }

    [JsonPropertyName("realtimeInputConfig")]
    public RealtimeInputConfig? RealtimeInputConfig { get; init; }

    [JsonPropertyName("sessionResumption")]
    public SessionResumptionConfig? SessionResumption { get; init; }

    [JsonPropertyName("contextWindowCompression")]
    public ContextWindowCompressionConfig? ContextWindowCompression { get; init; }

    [JsonPropertyName("inputAudioTranscription")]
    public AudioTranscriptionConfig? InputAudioTranscription { get; init; }

    [JsonPropertyName("outputAudioTranscription")]
    public AudioTranscriptionConfig? OutputAudioTranscription { get; init; }

    [JsonPropertyName("proactivity")]
    public ProactivityConfig? Proactivity { get; init; }
}

public record BidiGenerateContentSetupComplete;

public record BidiGenerateContentToolCall
{
    [JsonPropertyName("functionCalls")]
    public FunctionCall[]? FunctionCalls { get; init; }
}

public record BidiGenerateContentToolCallCancellation
{
    [JsonPropertyName("ids")]
    public string[]? Ids { get; init; }
}

public record BidiGenerateContentToolResponse
{
    [JsonPropertyName("functionResponses")]
    public FunctionResponse[]? FunctionResponses { get; init; }
}

public record BidiGenerateContentTranscription
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

public record ContextWindowCompressionConfig
{
    [JsonPropertyName("slidingWindow")]
    public SlidingWindow? SlidingWindow { get; init; }

    [JsonPropertyName("triggerTokens")]
    public long? TriggerTokens { get; init; }
}

public enum EndSensitivity
{
    [JsonStringEnumMemberName("END_SENSITIVITY_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("END_SENSITIVITY_HIGH")]
    High,

    [JsonStringEnumMemberName("END_SENSITIVITY_LOW")]
    Low,
}

public record GoAway
{
    [JsonPropertyName("timeLeft")]
    public string? TimeLeft { get; init; }
}

public record ProactivityConfig
{
    [JsonPropertyName("proactiveAudio")]
    public bool? ProactiveAudio { get; init; }
}

public record RealtimeInputConfig
{
    [JsonPropertyName("automaticActivityDetection")]
    public AutomaticActivityDetection? AutomaticActivityDetection { get; init; }

    [JsonPropertyName("activityHandling")]
    public ActivityHandling? ActivityHandling { get; init; }

    [JsonPropertyName("turnCoverage")]
    public TurnCoverage? TurnCoverage { get; init; }
}

public record SessionResumptionConfig
{
    [JsonPropertyName("handle")]
    public string? Handle { get; init; }
}

public record SessionResumptionUpdate
{
    [JsonPropertyName("newHandle")]
    public string? NewHandle { get; init; }

    [JsonPropertyName("resumable")]
    public bool? Resumable { get; init; }
}

public record SlidingWindow
{
    [JsonPropertyName("targetTokens")]
    public required long TargetTokens { get; init; }
}

public enum StartSensitivity
{
    [JsonStringEnumMemberName("START_SENSITIVITY_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("START_SENSITIVITY_HIGH")]
    High,

    [JsonStringEnumMemberName("START_SENSITIVITY_LOW")]
    Low,
}

public enum TurnCoverage
{
    [JsonStringEnumMemberName("TURN_COVERAGE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("TURN_INCLUDES_ONLY_ACTIVITY")]
    IncludesOnlyActivity,

    [JsonStringEnumMemberName("TURN_INCLUDES_ALL_INPUT")]
    IncludesAllInput,
}

public record UrlContextMetadata
{
    [JsonPropertyName("urlMetadata")]
    public required UrlMetadata UrlMetadata { get; init; }
}

public record UrlMetadata
{
    [JsonPropertyName("retrievedUrl")]
    public string? RetrievedUrl { get; init; }

    [JsonPropertyName("urlRetrievalStatus")]
    public UrlRetrievalStatus? UrlRetrievalStatus { get; init; }
}

public enum UrlRetrievalStatus
{
    [JsonStringEnumMemberName("URL_RETRIEVAL_STATUS_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("URL_RETRIEVAL_STATUS_SUCCESS")]
    Success,

    [JsonStringEnumMemberName("URL_RETRIEVAL_STATUS_ERROR")]
    Error,
}

public record LiveUsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int? PromptTokenCount { get; init; }

    [JsonPropertyName("cachedContentTokenCount")]
    public int? CachedContentTokenCount { get; init; }

    [JsonPropertyName("responseTokenCount")]
    public int? ResponseTokenCount { get; init; }

    [JsonPropertyName("toolUsePromptTokenCount")]
    public int? ToolUsePromptTokenCount { get; init; }

    [JsonPropertyName("thoughtsTokenCount")]
    public int? ThoughtsTokenCount { get; init; }

    [JsonPropertyName("totalTokenCount")]
    public int? TotalTokenCount { get; init; }

    [JsonPropertyName("promptTokensDetails")]
    public ModalityTokenCount[]? PromptTokensDetails { get; init; }

    [JsonPropertyName("cacheTokensDetails")]
    public ModalityTokenCount[]? CacheTokensDetails { get; init; }

    [JsonPropertyName("responseTokensDetails")]
    public ModalityTokenCount[]? ResponseTokensDetails { get; init; }

    [JsonPropertyName("toolUsePromptTokensDetails")]
    public ModalityTokenCount[]? ToolUsePromptTokensDetails { get; init; }
}

public record CreateAuthTokenRequest
{
    [JsonPropertyName("authToken")]
    public required AuthToken AuthToken { get; init; }
}

public record AuthToken
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("expireTime")]
    public string? ExpireTime { get; init; }

    [JsonPropertyName("newSessionExpireTime")]
    public string? NewSessionExpireTime { get; init; }

    [JsonPropertyName("bidiGenerateContentSetup")]
    public BidiGenerateContentSetup? BidiGenerateContentSetup { get; init; }

    [JsonPropertyName("uses")]
    public int? Uses { get; init; }
}