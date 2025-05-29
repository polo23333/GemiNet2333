namespace GemiNet;

public interface ILive
{
    Task<ILiveSession> ConnectAsync(BidiGenerateContentSetup request, CancellationToken cancellationToken = default);
}