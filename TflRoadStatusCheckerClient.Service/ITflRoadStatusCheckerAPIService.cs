using System.Threading;
using System.Threading.Tasks;
using TflRoadStatusCheckerClient.Contract.Models;

namespace TflRoadStatusCheckerClient.Service
{
    public interface ITflRoadStatusCheckerAPIService<T>
    {
        Task<TflRoadStatusCheckerResponse> GetRoadStatusAsync(string roadName, CancellationToken token);
    }
}