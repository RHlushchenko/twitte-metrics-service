using Models.Twitter;

namespace BusinessLogic.Interfaces
{
    public interface ITwitterService
    {
        TwitterMetrics GetMetrics();
    }
}
