namespace GemiNet;

public static class GenerateContentResponseExtensions
{
    public static string? GetText(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return null;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content == null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                if (part.Text != null) return part.Text;
            }
        }

        return null;
    }

    public static Blob? GetData(this GenerateContentResponse response)
    {
        if (response.Candidates == null) return null;

        foreach (var candidate in response.Candidates)
        {
            if (candidate.Content == null) continue;

            foreach (var part in candidate.Content.Parts)
            {
                if (part?.InlineData != null) return part.InlineData;
            }
        }

        return null;
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
}