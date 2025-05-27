using System.Text.Json.Serialization;

namespace GemiNet;

public record EmbedContentRequest
{
    public required string Model { get; init; }
    public required Contents Contents { get; init; }
    public TaskType? TaskType { get; init; }
    public string? Title { get; init; }
    public int? OutputDimensionality { get; init; }
}

public record EmbedContentRequestJsonData
{
    [JsonPropertyName("content")]
    public required Content Content { get; init; }

    [JsonPropertyName("taskType")]
    public TaskType? TaskType { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("outputDimensionality")]
    public int? OutputDimensionality { get; init; }
}

public record BatchEmbedContentRequestJsonData
{
    [JsonPropertyName("requests")]
    public required EmbedContentRequestJsonData[] Requests { get; init; }
}

public record EmbedContentResponse
{
    [JsonPropertyName("embeddings")]
    public required ContentEmbedding[] Embeddings { get; init; }
}

public record ContentEmbedding
{
    [JsonPropertyName("values")]
    public required double[] Values { get; init; }
}

public enum TaskType
{
    [JsonStringEnumMemberName("TASK_TYPE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("RETRIEVAL_QUERY")]
    RetrievalQuery,

    [JsonStringEnumMemberName("RETRIEVAL_DOCUMENT")]
    RetrievalDocument,

    [JsonStringEnumMemberName("SEMANTIC_SIMILARITY")]
    SemanticSimilarity,

    [JsonStringEnumMemberName("CLASSIFICATION")]
    Classification,

    [JsonStringEnumMemberName("CLUSTERING")]
    Clustering,

    [JsonStringEnumMemberName("QUESTION_ANSWERING")]
    QuestionAnswering,

    [JsonStringEnumMemberName("FACT_VERIFICATION")]
    FactVerification,

    [JsonStringEnumMemberName("CODE_RETRIEVAL_QUERY")]
    CodeRetrievalQuery,
}