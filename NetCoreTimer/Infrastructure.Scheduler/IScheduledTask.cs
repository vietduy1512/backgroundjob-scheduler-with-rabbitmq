using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Scheduler
{
    public interface IScheduledTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
