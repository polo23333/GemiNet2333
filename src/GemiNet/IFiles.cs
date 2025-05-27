using System.Runtime.CompilerServices;

namespace GemiNet;

public interface IFiles
{
    Task<File> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default);
    Task<File> GetAsync(GetFileRequest request, CancellationToken cancellationToken = default);
    Task<ListFilesResponse> ListAsync(ListFilesRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default);
}

public static class FilesExtensions
{
    public static async IAsyncEnumerable<File> ListAsync(this IFiles files, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new ListFilesRequest();

        while (true)
        {
            var response = await files.ListAsync(request, cancellationToken);
            foreach (var file in response.Files)
            {
                yield return file;
            }

            if (response.NextPageToken == null) break;
            request = request with
            {
                PageToken = response.NextPageToken,
            };
        }
    }
}