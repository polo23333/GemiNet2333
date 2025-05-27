using System.Runtime.CompilerServices;

namespace GemiNet;

public interface ICaches
{
    Task<CachedContent> CreateAsync(CreateChachedContentRequest request, CancellationToken cancellationToken = default);
    Task<ListCachedContentsResponse> ListAsync(ListCachedContentsRequest request, CancellationToken cancellationToken = default);
    Task<CachedContent> GetAsync(GetChachedContentRequest request, CancellationToken cancellationToken = default);
    Task<CachedContent> PatchAsync(PatchChachedContentRequest request, CancellationToken cancellationToken = default);
}

public static class CachesExtensions
{
    public static async IAsyncEnumerable<CachedContent> ListAsync(this ICaches caches, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new ListCachedContentsRequest();

        while (true)
        {
            var response = await caches.ListAsync(request, cancellationToken);
            foreach (var content in response.CachedContents ?? [])
            {
                yield return content;
            }

            if (response.NextPageToken == null) break;

            request = request with
            {
                PageToken = response.NextPageToken,
            };
        }
    }
}