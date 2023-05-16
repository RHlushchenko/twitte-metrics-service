using Models.Twitter;

namespace BusinessLogic.Interfaces
{
    public interface ITwitterClient
    {
        IAsyncEnumerable<TwitterResponse> GetTweetsStream(CancellationToken cancellationToken = default);
    }
}
