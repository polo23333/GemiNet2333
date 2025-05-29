using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.AI;

namespace GemiNet.Extensions.AI;

sealed class GoogleGenAIChatClient(GoogleGenAI ai, string model, IServiceProvider? serviceProvider = null) : IChatClient
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

        return CreateChatResponse(response, DateTime.Now);
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
                Role = CreateChatRole(candidate.Content.Role),
                Contents = [.. candidate.Content.Parts.Select(CreateAIContent)],
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

                systemInstruction = CreateContent(message);
                continue;
            }

            contents.Add(CreateContent(message));
        }

        return new()
        {
            Model = options?.ModelId ?? model,
            SystemInstruction = systemInstruction,
             Tools = CreateTools(options?.Tools),
            Contents = contents,
            GenerationConfig = CreateGenerateConfig(options),
        };
    }

    static string? CreateRole(ChatRole role)
    {
        if (role == ChatRole.System) return null;
        if (role == ChatRole.User) return "user";
        if (role == ChatRole.Assistant) return "model";
        if (role == ChatRole.Tool) return "model";

        throw new GemiNetException($"Unsupported role: '{role}'");
    }

    static ChatRole CreateChatRole(string? role)
    {
        if (role is null)
        {
            return ChatRole.System;
        }

        if (string.Equals(role, "user", StringComparison.OrdinalIgnoreCase))
        {
            return ChatRole.User;
        }

        if (string.Equals(role, "model", StringComparison.OrdinalIgnoreCase))
        {
            return ChatRole.Assistant;
        }

        throw new GemiNetException($"Unsupported role: {role}");
    }

    static Content CreateContent(ChatMessage chatMessage)
    {
        return new Content
        {
            Role = CreateRole(chatMessage.Role),
            Parts = [.. chatMessage.Contents.Select(CreatePart)],
        };
    }

    static Part CreatePart(AIContent content)
    {
        return content switch
        {
            TextContent textContent => new Part
            {
                Text = textContent.Text
            },
            DataContent dataContent => new Part
            {
                InlineData = new Blob
                {
                    Data = Convert.ToBase64String(dataContent.Data.Span),
                    MimeType = dataContent.MediaType,
                }
            },
            UriContent uriContent => new Part
            {
                FileData = new FileData
                {
                    FileUri = uriContent.Uri.AbsoluteUri,
                    MimeType = uriContent.MediaType,
                }
            },
            FunctionCallContent functionCall => new Part
            {
                FunctionCall = new FunctionCall
                {
                    Id = functionCall.CallId,
                    Name = functionCall.Name,
                    Args = JsonSerializer.SerializeToElement(functionCall.Arguments),
                }
            },
            FunctionResultContent functionResult => new Part
            {
                FunctionResponse = new FunctionResponse
                {
                    Id = functionResult.CallId,
                    Name = functionResult.CallId,
                    Response = JsonSerializer.SerializeToElement(new
                    {
                        content = JsonSerializer.SerializeToElement(functionResult.Result),
                    }),
                }
            },
            _ => throw new GemiNetException($"Unsupprted AIContent type: {content.GetType()}")
        };
    }

    static AIContent CreateAIContent(Part part)
    {
        if (part.Text is not null)
        {
            if (part.Thought == true)
            {
                return new TextReasoningContent(part.Text)
                {
                    RawRepresentation = part.Text,
                };
            }

            return new TextContent(part.Text)
            {
                RawRepresentation = part.Text,
            };
        }

        if (part.InlineData is not null)
        {
            return new DataContent(part.InlineData.Data, part.InlineData.MimeType)
            {
                RawRepresentation = part.InlineData,
            };
        }

        if (part.FunctionCall is not null)
        {
            var callId = part.FunctionCall.Id ?? $"{part.FunctionCall.Name}-{Guid.NewGuid()}";

            return new FunctionCallContent(callId, part.FunctionCall.Name,
                JsonSerializer.Deserialize<Dictionary<string, object?>>(part.FunctionCall.Args.GetValueOrDefault()))
            {
                RawRepresentation = part.FunctionCall,
            };
        }

        if (part.FunctionResponse is not null)
        {
            var responseId = part.FunctionResponse.Id ?? $"{part.FunctionResponse.Name}-{Guid.NewGuid()}";

            return new FunctionResultContent(responseId, part.FunctionResponse.Response)
            {
                RawRepresentation = part.FunctionResponse,
            };
        }

        if (part.FileData is not null)
        {
            return new DataContent(part.FileData.FileUri, part.FileData.MimeType)
            {
                RawRepresentation = part.FileData,
            };
        }

        if (part.ExecutableCode is not null)
        {
            return new AIContent
            {
                RawRepresentation = part.ExecutableCode,
            };
        }

        if (part.CodeExecutionResult is not null)
        {
            return new AIContent
            {
                RawRepresentation = part.CodeExecutionResult,
            };
        }

        throw new GemiNetException($"All properties of Part are null.");
    }

    static GenerationConfig? CreateGenerateConfig(ChatOptions? options)
    {
        if (options is null) return null;

        ThinkingConfig? thinkingConfig = null;

        if (options.AdditionalProperties?.TryGetValue("thinkingConfig", out var thinkingConfigObj) is true
            && thinkingConfigObj is ThinkingConfig obj)
        {
            thinkingConfig = obj;
        }

        var configuration = new GenerationConfig
        {
            StopSequences = options.StopSequences?.ToArray(),
            ResponseMimeType = options.ResponseFormat is ChatResponseFormatJson
                ? "application/json"
                : null,
            ResponseSchema = CreateSchema(options.ResponseFormat),
            MaxOutputTokens = options.MaxOutputTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            TopK = options.TopK,
            Seed = options.Seed == null ? null : (int)options.Seed.Value,
            PresencePenalty = options.PresencePenalty,
            FrequencyPenalty = options.FrequencyPenalty,
            ThinkingConfig = thinkingConfig
        };

        return configuration;
    }

    static Tool[]? CreateTools(IList<AITool>? tools)
    {
        if (tools is null) return null;

        var toolList = new List<Tool>(tools.Count);
        List<FunctionDeclaration>? functionDeclarations = null;

        foreach (var tool in tools)
        {
            if (tool is HostedCodeInterpreterTool)
            {
                toolList.Add(new Tool
                {
                    CodeExecution = new CodeExecution()
                });
                continue;
            }

            if (tool is AIFunction function)
            {
                functionDeclarations ??= [];

                functionDeclarations.Add(new FunctionDeclaration
                {
                    Name = function.Name,
                    Description = function.Description,
                    Parameters = CreateFunctionParameters(function.JsonSchema),
                    Response = null,
                });

                continue;
            }

            throw new GemiNetException($"Unsupported tool type: {tool.GetType()}");
        }

        if (functionDeclarations is not null)
        {
            toolList.Add(new Tool
            {
                FunctionDeclarations = functionDeclarations.ToArray()
            });
        }

        return toolList.ToArray();
    }

    static Schema? CreateSchema(ChatResponseFormat? responseFormat)
    {
        if (responseFormat is null) return null;

        if (responseFormat is ChatResponseFormatJson { Schema: JsonElement schema })
        {
            return Schema.FromJsonElement(schema);
        }

        if (responseFormat is ChatResponseFormatText) return null;

        throw new GemiNetException($"Unsupported response format: '{responseFormat}'");
    }

    static Schema? CreateFunctionParameters(JsonElement functionSchema)
    {
        var properties = functionSchema.GetProperty("properties");

        if (properties.ValueKind != JsonValueKind.Object)
        {
            throw new GemiNetException($"Expected object but got {properties.ValueKind}");
        }

        Dictionary<string, Schema>? parameters = null;

        foreach (var param in properties.EnumerateObject())
        {
            parameters ??= [];
            parameters[param.Name] = Schema.FromJsonElement(param.Value);
        }

        if (parameters is null) return null;

        return new Schema
        {
            Type = DataType.Unspecified,
            Properties = parameters,
        };
    }

    static ChatResponse CreateChatResponse(GenerateContentResponse response, DateTimeOffset createdAt)
    {
        var choices = response.Candidates?
            .Select(x => new ChatMessage
            {
                AuthorName = null,
                Role = CreateChatRole(x.Content?.Role),
                Contents = [.. x.Content?.Parts.Select(CreateAIContent) ?? []],
                RawRepresentation = x,
                AdditionalProperties = null
            })
            .ToArray();

        return new ChatResponse(choices)
        {
            ResponseId = null,
            ConversationId = null,
            ModelId = response.ModelVersion,
            CreatedAt = createdAt,
            FinishReason = response.Candidates?[0].FinishReason switch
            {
                FinishReason.Stop => ChatFinishReason.Stop,
                FinishReason.MaxTokens => ChatFinishReason.Length,
                FinishReason.Safety => ChatFinishReason.ContentFilter,
                _ => null
            },
            Usage = new UsageDetails
            {
                InputTokenCount = response.UsageMetadata?.PromptTokenCount,
                OutputTokenCount = response.UsageMetadata?.CandidatesTokenCount,
                TotalTokenCount = response.UsageMetadata?.TotalTokenCount,
                AdditionalCounts = null,
            },
            RawRepresentation = response,
            AdditionalProperties = null
        };
    }
}