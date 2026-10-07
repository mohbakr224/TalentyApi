using System;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Background
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
