using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliLoop.Core.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        public Task QueueAsync(Func<Task> work);
        public Task<Func<Task>> DequeueAsync();
    }
}
