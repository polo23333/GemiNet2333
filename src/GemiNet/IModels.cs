using System.Runtime.CompilerServices;

namespace GemiNet;

public interface IModels
{
    Task<Model> GetAsync(string name, CancellationToken cancellationToken = default);
    Task<ListModelsResponse> ListAsync(ListModelsRequest request, CancellationToken cancellationToken = default);
    Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<GenerateContentResponse> StreamGenerateContentAsync(GenerateContentRequest request, CancellationToken cancellationToken = default);

    Task<CountTokensResponse> CountTokensAsync(CountTokensRequest request, CancellationToken cancellationToken = default);

    Task<EmbedContentResponse> EmbedContentAsync(EmbedContentRequest request, CancellationToken cancellationToken = default);
}

public static class ModelsExtensions
{
    public static async IAsyncEnumerable<Model> ListAsync(this IModels models, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new ListModelsRequest();

        while (true)
        {
            var response = await models.ListAsync(request, cancellationToken);
            foreach (var model in response.Models)
            {
                yield return model;
            }

            if (response.NextPageToken == null) break;
            request = request with
            {
                PageToken = response.NextPageToken,
            };
        }
    }
}