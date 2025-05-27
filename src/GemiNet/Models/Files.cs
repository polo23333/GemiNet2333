using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace GemiNet;

public record UploadFileRequest
{
    public required UploadFileContent File { get; init; }
    public string? Name { get; init; }
    public string? MimeType { get; init; }
    public string? DisplayName { get; init; }
}

public record UploadFileResponse
{
    [JsonPropertyName("file")]
    public required File File { get; init; }
}

public record GetUploadUrlRequest
{
    [JsonPropertyName("file")]
    public required File File { get; init; }
}

public record GetFileRequest
{
    public required string Name { get; init; }
}

public record ListFilesRequest
{
    public int? PageSize { get; init; }
    public string? PageToken { get; init; }
}

public record ListFilesResponse
{
    [JsonPropertyName("files")]
    public required File[] Files { get; init; }

    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; init; }
}


public record DeleteFileRequest
{
    public required string Name { get; init; }
}

public record DeleteFileResponse
{
    [JsonPropertyName("file")]
    public required File File { get; init; }
}

[StructLayout(LayoutKind.Auto)]
public readonly record struct UploadFileContent
{
    public UploadFileContentType Type { get; }
    public string? Path { get; }
    public Blob? Blob { get; }

    public UploadFileContent(string path)
    {
        Type = UploadFileContentType.Path;
        Path = path;
    }

    public UploadFileContent(Blob blob)
    {
        Type = UploadFileContentType.Blob;
        Blob = blob;
    }

    public static implicit operator UploadFileContent(string path) => new(path);
    public static implicit operator UploadFileContent(Blob blob) => new(blob);
}

public enum UploadFileContentType : byte
{
    Path,
    Blob
}

public record File
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; init; }

    [JsonPropertyName("sizeBytes")]
    public string? SizeBytes { get; init; }

    [JsonPropertyName("createTime")]
    public string? CreateTime { get; init; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; init; }

    [JsonPropertyName("expirationTime")]
    public string? ExpirationTime { get; init; }

    [JsonPropertyName("sha256Hash")]
    public string? Sha256Hash { get; init; }

    [JsonPropertyName("uri")]
    public string? Uri { get; init; }

    [JsonPropertyName("downloadUri")]
    public string? DownloadUri { get; init; }

    [JsonPropertyName("state")]
    public FileState? State { get; init; }

    [JsonPropertyName("source")]
    public FileSource? Source { get; init; }

    [JsonPropertyName("videoMetadata")]
    public VideoFileMetadata? VideoMetadata { get; init; }
}

public record VideoFileMetadata
{
    [JsonPropertyName("videoDuration")]
    public string? VideoDuration { get; init; }
}

public enum FileState : byte
{
    [JsonStringEnumMemberName("STATE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("PROCESSING")]
    Processing,

    [JsonStringEnumMemberName("ACTIVE")]
    Active,

    [JsonStringEnumMemberName("FAILED")]
    Failed,
}

public enum FileSource : byte
{
    [JsonStringEnumMemberName("SOURCE_UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("UPLOADED")]
    Uploaded,

    [JsonStringEnumMemberName("GENERATED")]
    Generated,
}

public record FileStatus
{
    [JsonPropertyName("code")]
    public int? Code { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("details")]
    public Dictionary<string, string>[]? Details { get; init; }
}