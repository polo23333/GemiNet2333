using System.Text.Json.Serialization;

namespace GemiNet;

public record GenerateContentRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("contents")]
    public required Contents Contents { get; init; }

    [JsonPropertyName("tools")]
    public Tool[]? Tools { get; init; }

    [JsonPropertyName("toolConfig")]
    public ToolConfig? ToolConfig { get; init; }

    [JsonPropertyName("safetySettings")]
    public SafetySetting[]? SafetySettings { get; init; }

    [JsonPropertyName("systemInstruction")]
    public Content? SystemInstruction { get; init; }

    [JsonPropertyName("generationConfig")]
    public GenerationConfig? GenerationConfig { get; init; }
}

public record GenerateContentResponse
{
    [JsonPropertyName("candidates")]
    public Candidate[]? Candidates { get; init; }

    [JsonPropertyName("promptFeedback")]
    public GenerateContentResponsePromptFeedback? PromptFeedback { get; init; }

    [JsonPropertyName("usageMetadata")]
    public GenerateContentResponseUsageMetadata? UsageMetadata { get; init; }

    [JsonPropertyName("modelVersion")]
    public string? ModelVersion { get; init; }
}

public record GenerateContentResponsePromptFeedback
{
    [JsonPropertyName("blockReason")]
    public BlockReason? BlockReason { get; init; }

    [JsonPropertyName("safetyRatings")]
    public SafetyRating[]? SafetyRatings { get; init; }
}

public enum BlockReason
{
    [JsonStringEnumMemberName("BLOCK_REASON_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("SAFETY")]
    Safety,

    [JsonStringEnumMemberName("OTHER")]
    Other,

    [JsonStringEnumMemberName("BLOCKLIST")]
    BlockList,

    [JsonStringEnumMemberName("PROHIBITED_CONTENT")]
    ProhibitedContent,

    [JsonStringEnumMemberName("IMAGE_SAFETY")]
    ImageSafety,
}

public record GenerateContentResponseUsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int? PromptTokenCount { get; init; }

    [JsonPropertyName("cachedContentTokenCount")]
    public int? CachedContentTokenCount { get; init; }

    [JsonPropertyName("candidatesTokenCount")]
    public int? CandidatesTokenCount { get; init; }

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

    [JsonPropertyName("toolUsePromptTokensDetails")]
    public ModalityTokenCount[]? ToolUsePromptTokensDetails { get; init; }
}

public record Candidate
{
    [JsonPropertyName("content")]
    public Content? Content { get; init; }

    [JsonPropertyName("finishReason")]
    public FinishReason? FinishReason { get; init; }

    [JsonPropertyName("safetyRatings")]
    public SafetyRating[]? SafetyRatings { get; init; }

    [JsonPropertyName("citationMetadata")]
    public CitationMetadata? CitationMetadata { get; init; }

    [JsonPropertyName("tokenCount")]
    public int? TokenCount { get; init; }

    [JsonPropertyName("groundingAttributions")]
    public GroundingAttribution[]? GroundingAttributions { get; init; }

    [JsonPropertyName("groundingMetadata")]
    public GroundingAttribution? GroundingAttribution { get; init; }

    [JsonPropertyName("avgLogprobs")]
    public double? AvgLogprobs { get; init; }

    [JsonPropertyName("logprobsResult")]
    public LogprobsResult? LogprobsResult { get; init; }

    [JsonPropertyName("urlRetrievalMetadata")]
    public UrlRetrievalMetadata? UrlRetrievalMetadata { get; init; }

    [JsonPropertyName("index")]
    public int? Index { get; init; }
}

public enum FinishReason
{
    [JsonStringEnumMemberName("FINISH_REASON_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("STOP")]
    Stop,

    [JsonStringEnumMemberName("MAX_TOKENS")]
    MaxTokens,

    [JsonStringEnumMemberName("SAFETY")]
    Safety,

    [JsonStringEnumMemberName("RECITATION")]
    Recitation,

    [JsonStringEnumMemberName("LANGUAGE")]
    Language,

    [JsonStringEnumMemberName("OTHER")]
    Other,

    [JsonStringEnumMemberName("BLOCKLIST")]
    BlockList,

    [JsonStringEnumMemberName("PROHIBITED_CONTENT")]
    ProhibitedContent,

    [JsonStringEnumMemberName("SPII")]
    Spii,

    [JsonStringEnumMemberName("MALFORMED_FUNCTION_CALL")]
    MalformedFunctionCall,

    [JsonStringEnumMemberName("IMAGE_SAFETY")]
    ImageSafety,
}

public record GroundingAttribution
{
    [JsonPropertyName("sourceId")]
    public required AttributionSourceId SourceId { get; init; }

    [JsonPropertyName("content")]
    public required Content Content { get; init; }
}

public record AttributionSourceId
{
    [JsonPropertyName("groundingPassage")]
    public GroundingPassageId? GroundingPassage { get; init; }

    [JsonPropertyName("semanticRetrieverChunk")]
    public SemanticRetrieverChunk? SemanticRetrieverChunk { get; init; }
}

public record GroundingPassageId
{
    [JsonPropertyName("passageId")]
    public required string PassageId { get; init; }

    [JsonPropertyName("partIndex")]
    public required int PartIndex { get; init; }
}

public record SemanticRetrieverChunk
{
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    [JsonPropertyName("chunk")]
    public required string Chunk { get; init; }
}

public record GroundingMetadata
{
    [JsonPropertyName("groundingChunks")]
    public required GroundingChunk[] GroundingChunks { get; init; }

    [JsonPropertyName("groundingSupports")]
    public required GroundingSupport[] GroundingSupports { get; init; }

    [JsonPropertyName("webSearchQueries")]
    public required string[] WebSearchQueries { get; init; }

    [JsonPropertyName("searchEntryPoint")]
    public SearchEntryPoint? SearchEntryPoint { get; init; }

    [JsonPropertyName("retrievalMetadata")]
    public required RetrievalMetadata RetrievalMetadata { get; init; }
}

public record SearchEntryPoint
{
    [JsonPropertyName("renderedContent")]
    public string? RenderedContent { get; init; }

    [JsonPropertyName("sdkBlob")]
    public string? SdkBlob { get; init; }
}

public record GroundingChunk
{
    [JsonPropertyName("web")]
    public Web? Web { get; init; }
}

public record Web
{
    [JsonPropertyName("uri")]
    public required string Uri { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}

public record GroundingSupport
{
    [JsonPropertyName("groundingChunkIndices")]
    public required int[] GroundingChunkIndices { get; init; }

    [JsonPropertyName("confidenceScores")]
    public required double[] ConfidenceScores { get; init; }

    [JsonPropertyName("segment")]
    public required Segment Segment { get; init; }
}

public record Segment
{
    [JsonPropertyName("partIndex")]
    public required int PartIndex { get; init; }

    [JsonPropertyName("startIndex")]
    public required int StartIndex { get; init; }

    [JsonPropertyName("endIndex")]
    public required int EndIndex { get; init; }

    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

public record RetrievalMetadata
{
    [JsonPropertyName("googleSearchDynamicRetrievalScore")]
    public double? GoogleSearchDynamicRetrievalScore { get; init; }
}

public record LogprobsResult
{
    [JsonPropertyName("topCandidates")]
    public required LogprobsResultTopCandidates[] TopCandidates { get; init; }

    [JsonPropertyName("chosenCandidates")]
    public required LogprobsResultCandidate[] ChosenCandidates { get; init; }
}

public record LogprobsResultTopCandidates
{
    [JsonPropertyName("candidates")]
    public required LogprobsResultCandidate[] Candidates { get; init; }
}

public record LogprobsResultCandidate
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }

    [JsonPropertyName("tokenId")]
    public required int TokenId { get; init; }

    [JsonPropertyName("logProbability")]
    public required double LogProbability { get; init; }
}

public record UrlRetrievalMetadata
{
    [JsonPropertyName("urlRetrievalContexts")]
    public required UrlRetrievalContext[] UrlRetrievalContexts { get; init; }
}

public record UrlRetrievalContext
{
    [JsonPropertyName("retrievedUrl")]
    public required string RetrievedUrl { get; init; }
}

public record CitationMetadata
{
    [JsonPropertyName("citationSources")]
    public required CitationSource[] CitationSources { get; init; }
}

public record CitationSource
{
    [JsonPropertyName("startIndex")]
    public int? StartIndex { get; init; }

    [JsonPropertyName("endIndex")]
    public int? EndIndex { get; init; }

    [JsonPropertyName("uri")]
    public string? Uri { get; init; }

    [JsonPropertyName("license")]
    public string? License { get; init; }
}

public record GenerationConfig
{
    [JsonPropertyName("stopSequences")]
    public string[]? StopSequences { get; init; }

    [JsonPropertyName("responseMimeType")]
    public string? ResponseMimeType { get; init; }

    [JsonPropertyName("responseSchema")]
    public Schema? ResponseSchema { get; init; }

    [JsonPropertyName("responseModalities")]
    public Modality[]? ResponseModalities { get; init; }

    [JsonPropertyName("candidateCount")]
    public int? CandidateCount { get; init; }

    [JsonPropertyName("maxOutputTokens")]
    public int? MaxOutputTokens { get; init; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    [JsonPropertyName("topP")]
    public double? TopP { get; init; }

    [JsonPropertyName("topK")]
    public int? TopK { get; init; }

    [JsonPropertyName("seed")]
    public int? Seed { get; init; }

    [JsonPropertyName("presencePenalty")]
    public double? PresencePenalty { get; init; }

    [JsonPropertyName("frequencyPenalty")]
    public double? FrequencyPenalty { get; init; }

    [JsonPropertyName("responseLogprobs")]
    public bool? ResponseLogprobs { get; init; }

    [JsonPropertyName("logprobs")]
    public int? Logprobs { get; init; }

    [JsonPropertyName("enableEnhancedCivicAnswers")]
    public bool? EnableEnhancedCivicAnswers { get; init; }

    [JsonPropertyName("speechConfig")]
    public SpeechConfig? SpeechConfig { get; init; }

    [JsonPropertyName("thinkingConfig")]
    public ThinkingConfig? ThinkingConfig { get; init; }

    [JsonPropertyName("mediaResolution")]
    public MediaResolution? MediaResolution { get; init; }
}

public enum Modality
{
    [JsonStringEnumMemberName("MODALITY_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("TEXT")]
    Text,

    [JsonStringEnumMemberName("IMAGE")]
    Image,

    [JsonStringEnumMemberName("AUDIO")]
    Audio,
}

public record ModalityTokenCount
{
    [JsonPropertyName("modality")]
    public required Modality Modality { get; init; }

    [JsonPropertyName("tokenCount")]
    public required int TokenCount { get; init; }
}

public record SpeechConfig
{
    [JsonPropertyName("voiceConfig")]
    public required VoiceConfig VoiceConfig { get; init; }

    [JsonPropertyName("languageCode")]
    public string? LanguageCode { get; init; }
}

public record VoiceConfig
{
    [JsonPropertyName("prebuiltVoiceConfig")]
    public required PrebuiltVoiceConfig PrebuiltVoiceConfig { get; init; }
}

public record PrebuiltVoiceConfig
{
    [JsonPropertyName("voiceName")]
    public required string VoiceName { get; init; }
}

public record ThinkingConfig
{
    [JsonPropertyName("includeThoughts")]
    public required bool IncludeThoughts { get; init; }

    [JsonPropertyName("thinkingBudget")]
    public required int ThinkingBudget { get; init; }
}

public enum MediaResolution
{
    [JsonStringEnumMemberName("MEDIA_RESOLUTION_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("MEDIA_RESOLUTION_LOW")]
    Low,

    [JsonStringEnumMemberName("MEDIA_RESOLUTION_MEDIUM")]
    Medium,

    [JsonStringEnumMemberName("MEDIA_RESOLUTION_HIGH")]
    High,
}

public enum HarmCategory
{
    [JsonStringEnumMemberName("HARM_CATEGORY_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("HARM_CATEGORY_DEROGATORY")]
    Derogatory,

    [JsonStringEnumMemberName("HARM_CATEGORY_TOXICITY")]
    Toxicity,

    [JsonStringEnumMemberName("HARM_CATEGORY_VIOLENCE")]
    Violence,

    [JsonStringEnumMemberName("HARM_CATEGORY_SEXUAL")]
    Sexual,

    [JsonStringEnumMemberName("HARM_CATEGORY_MEDICAL")]
    Medical,

    [JsonStringEnumMemberName("HARM_CATEGORY_DANGEROUS")]
    Dangerous,

    [JsonStringEnumMemberName("HARM_CATEGORY_HARASSMENT")]
    Harassment,

    [JsonStringEnumMemberName("HARM_CATEGORY_HATE_SPEECH")]
    HateSpeech,

    [JsonStringEnumMemberName("HARM_CATEGORY_SEXUALLY_EXPLICIT")]
    SexuallyExplicit,

    [JsonStringEnumMemberName("HARM_CATEGORY_DANGEROUS_CONTENT")]
    DangerousContent,

    [JsonStringEnumMemberName("HARM_CATEGORY_CIVIC_INTEGRITY")]
    CivicIntegrity,
}

public record SafetyRating
{
    [JsonPropertyName("category")]
    public required HarmCategory Category { get; init; }

    [JsonPropertyName("probability")]
    public required HarmProbability Probability { get; init; }

    [JsonPropertyName("blocked")]
    public bool? Blocked { get; init; }
}

public enum HarmProbability
{
    [JsonStringEnumMemberName("HARM_PROBABILITY_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("NEGLIGIBLE")]
    Negligible,

    [JsonStringEnumMemberName("LOW")]
    Low,

    [JsonStringEnumMemberName("MEDIUM")]
    Medium,

    [JsonStringEnumMemberName("HIGH")]
    High,
}

public record SafetySetting
{
    [JsonPropertyName("category")]
    public required HarmCategory Category { get; init; }

    [JsonPropertyName("threshold")]
    public required HarmBlockThreshold Threshold { get; init; }
}

public enum HarmBlockThreshold
{
    [JsonStringEnumMemberName("HARM_BLOCK_THRESHOLD_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("BLOCK_LOW_AND_ABOVE")]
    BlockLowAndAbove,

    [JsonStringEnumMemberName("BLOCK_MEDIUM_AND_ABOVE")]
    BlockMediumAndAbove,

    [JsonStringEnumMemberName("BLOCK_ONLY_HIGH")]
    BlockOnlyHigh,

    [JsonStringEnumMemberName("BLOCK_NONE")]
    BlockNone,

    [JsonStringEnumMemberName("OFF")]
    Off,
}

