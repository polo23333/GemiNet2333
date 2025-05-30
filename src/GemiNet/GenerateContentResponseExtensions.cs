namespace GemiNet;

public static class GenerateContentResponseExtensions
{
    public static string? GetText(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return null;

        using var buffer = new PooledList<char>(1024);

        var anyTextPartFound = false;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content == null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                if (part.Text != null)
                {
                    buffer.AddRange(part.Text);
                    anyTextPartFound = true;
                }
            }
        }

        if (!anyTextPartFound) return null;

        return buffer.AsSpan().ToString();
    }

    public static byte[]? GetData(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return null;

        using var buffer = new PooledList<byte>(1024);
        var anyDataPartFound = false;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content == null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                if (part?.InlineData != null)
                {
                    buffer.AddRange(Convert.FromBase64String(part.InlineData.Data));
                    anyDataPartFound = true;
                }
            }
        }

        if (!anyDataPartFound) return null;

        return buffer.AsSpan().ToArray();
    }

    public static FunctionCall[] GetFunctionCalls(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return [];

        return response.Candidates
            .SelectMany(x => x.Content?.Parts ?? [])
            .Where(x => x.FunctionCall != null)
            .Select(x => x.FunctionCall)
            .ToArray()!;
    }

    public static ExecutableCode? GetExecutableCode(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return null;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content == null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                if (part?.ExecutableCode != null) return part.ExecutableCode;
            }
        }

        return null;
    }


    public static CodeExecutionResult? GetCodeExecutionResult(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return null;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content == null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                if (part?.CodeExecutionResult != null) return part.CodeExecutionResult;
            }
        }

        return null;
    }

    public static string? GetText(this BidiGenerateContentServerMessage message)
    {
        using var buffer = new PooledList<char>(1024);

        if (message.ServerContent == null ||
            message.ServerContent.ModelTurn == null ||
            message.ServerContent.ModelTurn.Parts.Length == 0)
        {
            return null;
        }

        var anyTextPartFound = false;

        foreach (var part in message.ServerContent.ModelTurn.Parts)
        {
            if (part.Text != null)
            {
                buffer.AddRange(part.Text);
                anyTextPartFound = true;
            }
        }

        if (!anyTextPartFound) return null;

        return buffer.AsSpan().ToString();
    }

    public static byte[]? GetData(this BidiGenerateContentServerMessage message)
    {
        using var buffer = new PooledList<byte>(1024);

        if (message.ServerContent == null ||
            message.ServerContent.ModelTurn == null ||
            message.ServerContent.ModelTurn.Parts.Length == 0)
        {
            return null;
        }

        var anyDataPartFound = false;

        foreach (var part in message.ServerContent.ModelTurn.Parts)
        {
            if (part.InlineData != null)
            {
                buffer.AddRange(Convert.FromBase64String(part.InlineData.Data));
                anyDataPartFound = true;
            }
        }

        if (!anyDataPartFound) return null;

        return buffer.AsSpan().ToArray();
    }
}
