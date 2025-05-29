using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace GemiNet;

public class GoogleGenAI : IDisposable
{
    sealed class GoogleGenAIModels(GoogleGenAI ai) : IModels
    {
        public async Task<Model> GetAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.GetAsync($"{ai.BaseUrl}/{name}?key={ai.ApiKey}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<Model>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task<ListModelsResponse> ListAsync(ListModelsRequest request, CancellationToken cancellationToken = default)
        {
            var queryParameters = (request.PageSize, request.PageToken) switch
            {
                (int size, string token) => $"key={ai.ApiKey}&pageSize={size}&pageToken={token}",
                (int size, null) => $"key={ai.ApiKey}&pageSize={size}",
                (null, string token) => $"key={ai.ApiKey}&pageToken={token}",
                (null, null) => $"key={ai.ApiKey}",
            };

            var response = await ai.HttpClient.GetAsync($"{ai.BaseUrl}/models?{queryParameters}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<ListModelsResponse>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.PostAsJsonAsync(
                $"{ai.BaseUrl}/{request.Model}:generateContent?key={ai.ApiKey}",
                request,
                GemiNetJsonSerializerContext.Default.GetTypeInfo<GenerateContentRequest>()!,
                cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<GenerateContentResponse>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async IAsyncEnumerable<GenerateContentResponse> StreamGenerateContentAsync(GenerateContentRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var response = await ai.HttpClient.PostAsJsonAsync(
                $"{ai.BaseUrl}/{request.Model}:streamGenerateContent?key={ai.ApiKey}",
                request,
                GemiNetJsonSerializerContext.Default.GetTypeInfo<GenerateContentRequest>()!,
                cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

#if NET6_0_OR_GREATER
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
#else
            var stream = await response.Content.ReadAsStreamAsync()
#endif
                .ConfigureAwait(ai.ConfigureAwait);

            await foreach (var result in JsonSerializer.DeserializeAsyncEnumerable(stream, GemiNetJsonSerializerContext.Default.GetTypeInfo<GenerateContentResponse>()!, cancellationToken))
            {
                if (result == null) continue;
                yield return result!;
            }
        }

        public async Task<CountTokensResponse> CountTokensAsync(CountTokensRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.PostAsJsonAsync(
                $"{ai.BaseUrl}/{request.Model}:countTokens?key={ai.ApiKey}",
                request,
                GemiNetJsonSerializerContext.Default.GetTypeInfo<CountTokensRequest>()!,
                cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<CountTokensResponse>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task<EmbedContentResponse> EmbedContentAsync(EmbedContentRequest request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            if (request.Contents.Count == 1)
            {
                response = await ai.HttpClient.PostAsJsonAsync(
                    $"{ai.BaseUrl}/{request.Model}:embedContent?key={ai.ApiKey}",
                    new()
                    {
                        Content = request.Contents[0],
                        OutputDimensionality = request.OutputDimensionality,
                        TaskType = request.TaskType,
                        Title = request?.Title,
                    },
                    GemiNetJsonSerializerContext.Default.GetTypeInfo<EmbedContentRequestJsonData>()!,
                    cancellationToken)
                    .ConfigureAwait(ai.ConfigureAwait);
            }
            else
            {
                var requests = new EmbedContentRequestJsonData[request.Contents.Count];
                for (int i = 0; i < requests.Length; i++)
                {
                    requests[i] = new()
                    {
                        Content = request!.Contents[i],
                        OutputDimensionality = request.OutputDimensionality,
                        TaskType = request.TaskType,
                        Title = request?.Title,
                    };
                }

                response = await ai.HttpClient.PostAsJsonAsync(
                    $"{ai.BaseUrl}/{request!.Model}:batchEmbedContents?key={ai.ApiKey}",
                    new()
                    {
                        Requests = requests,
                    },
                    GemiNetJsonSerializerContext.Default.GetTypeInfo<BatchEmbedContentRequestJsonData>()!,
                    cancellationToken)
                    .ConfigureAwait(ai.ConfigureAwait);
            }

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<EmbedContentResponse>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }
    }

    // TODO: optimize
    sealed class GoogleGenAIFiles(GoogleGenAI ai) : IFiles
    {
        public Task<File> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default)
        {
            return request.File.Type switch
            {
                UploadFileContentType.Path => UploadFromFilePathAsync(request, request.File.Path!, cancellationToken),
                UploadFileContentType.Blob => UploadBlobAsync(request, request.File.Blob!, cancellationToken),
                _ => default!,
            };
        }

        async Task<string> FetchUploadUrlAsync(File fileToUpload, CancellationToken cancellationToken)
        {
            var uri = $"https://generativelanguage.googleapis.com/upload/v1beta/files?key={ai.ApiKey}";
            var metadata = new GetUploadUrlRequest
            {
                File = fileToUpload,
            };

            var metadataJson = JsonSerializer.Serialize(metadata, GemiNetJsonSerializerContext.Default.GetTypeInfo<GetUploadUrlRequest>()!);
            var metadataContent = new StringContent(metadataJson, Encoding.UTF8, "application/json");
            metadataContent.Headers.Add("X-Goog-Upload-Protocol", "resumable");
            metadataContent.Headers.Add("X-Goog-Upload-Command", "start");
            metadataContent.Headers.Add("X-Goog-Upload-Header-Content-Length", fileToUpload.SizeBytes);
            metadataContent.Headers.Add("X-Goog-Upload-Header-Content-Type", fileToUpload.MimeType);

            using var metaRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = metadataContent
            };

            var metaResponse = await ai.HttpClient.SendAsync(metaRequest, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!metaResponse.IsSuccessStatusCode)
            {
                throw new GemiNetException("Upload URL not found");
            }

            if (!metaResponse.Headers.TryGetValues("X-Goog-Upload-URL", out var uploadUrls))
            {
                throw new GemiNetException("Upload URL not found.");
            }

            return uploadUrls.First();
        }

        async Task<File> UploadFromFilePathAsync(UploadFileRequest request, string filePath, CancellationToken cancellationToken)
        {
            var fileSize = new FileInfo(filePath).Length;

            var metadata = new File()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                MimeType = request.MimeType ?? Mime.GetMimeType(filePath),
                SizeBytes = fileSize.ToString(),
            };

            var uploadUrl = await FetchUploadUrlAsync(metadata, cancellationToken);

            using var content = new StreamContent(System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read));
            content.Headers.ContentLength = fileSize;
            content.Headers.ContentType = new MediaTypeHeaderValue(metadata.MimeType);
            content.Headers.Add("X-Goog-Upload-Command", "upload, finalize");
            content.Headers.Add("X-Goog-Upload-Offset", "0");

            var response = await ai.HttpClient.PostAsync(uploadUrl, content, cancellationToken);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

#if NET6_0_OR_GREATER
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
#else
            var stream = await response.Content.ReadAsStreamAsync()
#endif
                .ConfigureAwait(ai.ConfigureAwait);

            var file = (await JsonSerializer.DeserializeAsync(stream, GemiNetJsonSerializerContext.Default.GetTypeInfo<UploadFileResponse>()!, cancellationToken)!
                .ConfigureAwait(ai.ConfigureAwait))!.File;

            if (response.Headers.TryGetValues("X-Goog-Upload-Status", out var finalStatusValues))
            {
                if (finalStatusValues.FirstOrDefault() != "final")
                {
                    throw new GemiNetException("Failed to upload file: Upload status is not finalized.");
                }
            }

            return file!;
        }

        async Task<File> UploadBlobAsync(UploadFileRequest request, Blob blob, CancellationToken cancellationToken)
        {
            var blobBytes = Convert.FromBase64String(blob.Data);
            var blobSize = blobBytes.Length;

            var metadata = new File()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                MimeType = request.MimeType,
                SizeBytes = blobSize.ToString(),
            };

            var uploadUrl = await FetchUploadUrlAsync(metadata, cancellationToken);

            using var content = new ByteArrayContent(blobBytes);
            content.Headers.ContentLength = blobSize;
            content.Headers.ContentType = new(metadata.MimeType!);
            content.Headers.Add("X-Goog-Upload-Command", "upload, finalize");
            content.Headers.Add("X-Goog-Upload-Offset", "0");

            var response = await ai.HttpClient.PostAsync(uploadUrl, content, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var file = (await JsonSerializer.DeserializeAsync(
#if NET6_0_OR_GREATER
                await response.Content.ReadAsStreamAsync(cancellationToken),
#else
                await response.Content.ReadAsStreamAsync(),
#endif
                GemiNetJsonSerializerContext.Default.GetTypeInfo<UploadFileResponse>()!,
                cancellationToken)!
                .ConfigureAwait(ai.ConfigureAwait))!.File;

            if (response.Headers.TryGetValues("X-Goog-Upload-Status", out var finalStatusValues))
            {
                if (finalStatusValues.FirstOrDefault() != "final")
                {
                    throw new GemiNetException("Failed to upload file: Upload status is not finalized.");
                }
            }

            return file!;
        }

        public async Task<File> GetAsync(GetFileRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.GetAsync($"{ai.BaseUrl}/{request.Name}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<File>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task<ListFilesResponse> ListAsync(ListFilesRequest request, CancellationToken cancellationToken = default)
        {
            var queryParameters = (request.PageSize, request.PageToken) switch
            {
                (int size, string token) => $"key={ai.ApiKey}&pageSize={size}&pageToken={token}",
                (int size, null) => $"key={ai.ApiKey}&pageSize={size}",
                (null, string token) => $"key={ai.ApiKey}&pageToken={token}",
                (null, null) => $"key={ai.ApiKey}",
            };

            var response = await ai.HttpClient.GetAsync($"{ai.BaseUrl}/files?{queryParameters}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<ListFilesResponse>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.DeleteAsync($"{ai.BaseUrl}/{request.Name}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);
        }
    }

    sealed class GoogleGenAICaches(GoogleGenAI ai) : ICaches
    {
        public async Task<CachedContent> CreateAsync(CreateChachedContentRequest request, CancellationToken cancellationToken = default)
        {
            var content = new CachedContent
            {
                Model = request.Model,
                Contents = request.Contents,
                Tools = request.Tools,
                ExpireTime = request.ExpireTime,
                Ttl = request.Ttl,
                DisplayName = request.DisplayName,
                SystemInstruction = request.SystemInstruction,
                ToolConfig = request.ToolConfig,
            };

            var response = await ai.HttpClient.PostAsJsonAsync($"{ai.BaseUrl}/cachedContents?key={ai.ApiKey}", content, GemiNetJsonSerializerContext.Default.GetTypeInfo<CachedContent>()!, cancellationToken);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<CachedContent>()!, cancellationToken);
            return result!;
        }

        public async Task<ListCachedContentsResponse> ListAsync(ListCachedContentsRequest request, CancellationToken cancellationToken = default)
        {
            var queryParameters = (request.PageSize, request.PageToken) switch
            {
                (int size, string token) => $"key={ai.ApiKey}&pageSize={size}&pageToken={token}",
                (int size, null) => $"key={ai.ApiKey}&pageSize={size}",
                (null, string token) => $"key={ai.ApiKey}&pageToken={token}",
                (null, null) => $"key={ai.ApiKey}",
            };

            var response = await ai.HttpClient.GetAsync($"{ai.BaseUrl}/cachedContents?{queryParameters}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<ListCachedContentsResponse>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task<CachedContent> GetAsync(GetChachedContentRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.GetAsync($"{ai.BaseUrl}/cachedContents?{request.Name}?key={ai.ApiKey}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<CachedContent>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task<CachedContent> PatchAsync(PatchChachedContentRequest request, CancellationToken cancellationToken = default)
        {
            var url = $"{ai.BaseUrl}/cachedContents?{request.Name}?key={ai.ApiKey}{(request.UpdateMask == null ? "" : $"?updateMask={request.UpdateMask}")}";
            var response = await ai.HttpClient.PatchAsJsonAsync(url, request.Config, GemiNetJsonSerializerContext.Default.GetTypeInfo<PatchChachedContentConfig>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<CachedContent>()!, cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            return result!;
        }

        public async Task DeleteAsync(DeleteChachedContentRequest request, CancellationToken cancellationToken = default)
        {
            var response = await ai.HttpClient.DeleteAsync($"{ai.BaseUrl}/cachedContents?{request.Name}?key={ai.ApiKey}", cancellationToken)
                .ConfigureAwait(ai.ConfigureAwait);

            if (!response.IsSuccessStatusCode) await ai.ThrowFromErrorResponseAsync(response, cancellationToken);
        }
    }

    sealed class GoogleGenAILive(GoogleGenAI ai) : ILive
    {
        public async Task<ILiveSession> ConnectAsync(BidiGenerateContentSetup request, CancellationToken cancellationToken = default)
        {
            var live = new LiveSession(ai);
            await live.ConnectAsync(request, cancellationToken);
            return live;
        }
    }

    public GoogleGenAI() : this(new HttpClientHandler(), true)
    {
    }

    public GoogleGenAI(HttpMessageHandler handler)
        : this(handler, true)
    {
    }

    public GoogleGenAI(HttpMessageHandler handler, bool disposeHandler)
    {
        HttpClient = new(handler, disposeHandler);
        Models = new GoogleGenAIModels(this);
        Files = new GoogleGenAIFiles(this);
        Caches = new GoogleGenAICaches(this);
        Live = new GoogleGenAILive(this);
    }

    public string ApiKey { get; set; } = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";

    public bool ConfigureAwait { get; set; } = false;

    public IModels Models { get; }
    public IFiles Files { get; }
    public ICaches Caches { get; }
    public ILive Live { get; }

    public HttpClient HttpClient { get; }

    public string BaseUrl { get; } = "https://generativelanguage.googleapis.com/v1beta";

    async Task ThrowFromErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var error = (await response.Content.ReadFromJsonAsync(GemiNetJsonSerializerContext.Default.GetTypeInfo<ErrorResponse>()!, cancellationToken).ConfigureAwait(ConfigureAwait))!.Error;
        throw new GemiNetException(error.Message);
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}