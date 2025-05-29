using Microsoft.Extensions.AI;

namespace GemiNet.Extensions.AI;

public static class GoogleGenAIExtensions
{
    public static IChatClient AsChatClient(this GoogleGenAI ai, string model)
    {
        return new GoogleGenAIChatClient(ai, model);
    }
}
