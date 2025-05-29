using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GemiNet;

public interface ILiveSession : IAsyncDisposable
{
    ValueTask ConnectAsync(BidiGenerateContentSetup request, CancellationToken cancellationToken = default);
    ValueTask SendClientContentAsync(BidiGenerateContentClientContent content, CancellationToken cancellationToken = default);
    ValueTask SendRealtimeInputAsync(BidiGenerateContentRealtimeInput input, CancellationToken cancellationToken = default);
    ValueTask SendToolResponseAsync(BidiGenerateContentToolResponse toolResponse, CancellationToken cancellationToken = default);
    IAsyncEnumerable<BidiGenerateContentServerMessage> ReceiveAsync(CancellationToken cancellationToken = default);
}

sealed class LiveSession(GoogleGenAI ai) : ILiveSession
{
    readonly ClientWebSocket socket = new();
    readonly CancellationTokenSource cts = new();

    public async ValueTask ConnectAsync(BidiGenerateContentSetup setup, CancellationToken cancellationToken = default)
    {
        var url = $"wss://generativelanguage.googleapis.com/ws/google.ai.generativelanguage.v1beta.GenerativeService.BidiGenerateContent?key={ai.ApiKey}";
        await socket.ConnectAsync(new Uri(url), cancellationToken);

        var json = JsonSerializer.SerializeToUtf8Bytes(new()
        {
            Setup = setup,
        }, GemiNetJsonSerializerContext.Default.GetTypeInfo<BidiGenerateContent>()!);

        await socket.SendAsync(json, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, cancellationToken);
    }

    public async ValueTask SendRealtimeInputAsync(BidiGenerateContentRealtimeInput realtimeInput, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(new()
        {
            RealtimeInput = realtimeInput
        }, GemiNetJsonSerializerContext.Default.GetTypeInfo<BidiGenerateContent>()!);

        await socket.SendAsync(json, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, cancellationToken);
    }

    public async ValueTask SendClientContentAsync(BidiGenerateContentClientContent clientContent, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(new()
        {
            ClientContent = clientContent,
        }, GemiNetJsonSerializerContext.Default.GetTypeInfo<BidiGenerateContent>()!);

        await socket.SendAsync(json, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, cancellationToken);
    }

    public async ValueTask SendToolResponseAsync(BidiGenerateContentToolResponse toolResponse, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(new()
        {
            ToolResponse = toolResponse,
        }, GemiNetJsonSerializerContext.Default.GetTypeInfo<BidiGenerateContent>()!);

        await socket.SendAsync(json, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, cancellationToken);
    }

    public async IAsyncEnumerable<BidiGenerateContentServerMessage> ReceiveAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (socket.State == WebSocketState.Open)
        {
            cts.Token.ThrowIfCancellationRequested();

            var buffer = ArrayPool<byte>.Shared.Rent(8192);
            var bytesConsumed = 0;
            try
            {
                ValueWebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(buffer.AsMemory(bytesConsumed), cts.Token);
                    bytesConsumed += result.Count;
                } while (!result.EndOfMessage);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            var msg = JsonSerializer.Deserialize(
                buffer.AsSpan(0, bytesConsumed),
                GemiNetJsonSerializerContext.Default.GetTypeInfo<BidiGenerateContentServerMessage>()!);

            if (msg != null) yield return msg;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (socket.State == WebSocketState.Open)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", cts.Token);
        }
        socket.Dispose();

        cts.Cancel();
        cts.Dispose();
    }
}