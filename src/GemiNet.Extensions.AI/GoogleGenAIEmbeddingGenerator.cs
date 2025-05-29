using Microsoft.Extensions.AI;

namespace GemiNet.Extensions.AI;

internal sealed class GoogleGenAIEmbeddingGenerator(GoogleGenAI ai, string model) : IEmbeddingGenerator<string, Embedding<float>>
{
    const int GeminiEmbeddingSize = 768;

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var request = new EmbedContentRequest
        {
            Model = model,
            Contents = new Content
            {
                Parts = [.. values.Select(v => new Part { Text = v })]
            },
            OutputDimensionality = options?.Dimensions,
        };

        var response = await ai.Models.EmbedContentAsync(request, cancellationToken);

        var embeddingSize = options?.Dimensions ?? GeminiEmbeddingSize;

        var vector = response.Embeddings
            .SelectMany(x => x.Values)
            .Select(x => (float)x)
            .ToArray();

        if (vector.Length % embeddingSize != 0)
        {
            throw new InvalidOperationException($"The returned embedding vector's size is not a multiple of the expected dimensions '{embeddingSize}'.");
        }

        var generatedEmbeddings = new GeneratedEmbeddings<Embedding<float>>(vector.Length / embeddingSize);

        for (int i = 0; i < vector.Length; i += embeddingSize)
        {
            var embedding = new Embedding<float>(vector.AsMemory().Slice(i, embeddingSize));
            generatedEmbeddings.Add(embedding);
        }

        return generatedEmbeddings;
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceType == typeof(GoogleGenAI))
        {
            return ai;
        }

        return null;
    }

    public void Dispose()
    {
    }
}