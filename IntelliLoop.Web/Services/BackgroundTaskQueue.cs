using IntelliLoop.Core.Interfaces;
using System.Threading.Channels;

namespace IntelliLoop.Web.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<Task>> _queue = Channel.CreateUnbounded<Func<Task>>();

        public async Task QueueAsync(Func<Task> work)
        {
            await _queue.Writer.WriteAsync(work);
        }

        public async Task<Func<Task>> DequeueAsync()
        {
            return await _queue.Reader.ReadAsync();
        }
    }
}
