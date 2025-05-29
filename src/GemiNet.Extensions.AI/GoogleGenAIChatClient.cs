using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace GemiNet.Extensions.AI;

internal sealed class GoogleGenAIChatClient(GoogleGenAI ai, string model, IServiceProvider? serviceProvider = null) : IChatClient
{
    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return serviceProvider?.GetService(serviceType);
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await ai.Models.GenerateContentAsync(
            CreateGenerateContentRequest(messages, options),
            cancellationToken);

        return ModelConverter.CreateChatResponse(response, DateTime.Now);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = CreateGenerateContentRequest(messages, options);

        await foreach (var response in ai.Models.StreamGenerateContentAsync(request, cancellationToken))
        {
            if (response.Candidates == null || response.Candidates.Length == 0) continue;

            var candidate = response.Candidates?[0];
            if (candidate == null || candidate.Content == null) continue;

            yield return new ChatResponseUpdate
            {
                AuthorName = null,
                Role = ModelConverter.CreateChatRole(candidate.Content.Role),
                Contents = [.. candidate.Content.Parts.Select(ModelConverter.CreateAIContent)],
                RawRepresentation = response,
                CreatedAt = DateTime.Now,
                FinishReason = candidate.FinishReason switch
                {
                    FinishReason.Stop => ChatFinishReason.Stop,
                    FinishReason.MaxTokens => ChatFinishReason.Length,
                    FinishReason.Safety => ChatFinishReason.ContentFilter,
                    _ => null
                },
                ModelId = response.ModelVersion,
            };
        }
    }

    public void Dispose()
    {
        ai.Dispose();
    }

    GenerateContentRequest CreateGenerateContentRequest(IEnumerable<ChatMessage> messages, ChatOptions? options)
    {
        Content? systemInstruction = null;
        Contents contents = [];

        foreach (var message in messages)
        {
            if (message.Role == ChatRole.System)
            {
                if (systemInstruction is not null)
                {
                    throw new GemiNetException("Cannot use multiple system instructions");
                }

                systemInstruction = ModelConverter.CreateContent(message);
                continue;
            }

            contents.Add(ModelConverter.CreateContent(message));
        }

        return new()
        {
            Model = options?.ModelId ?? model,
            SystemInstruction = systemInstruction,
            Tools = ModelConverter.CreateTools(options?.Tools),
            Contents = contents,
            GenerationConfig = ModelConverter.CreateGenerateConfig(options),
        };
    }

}