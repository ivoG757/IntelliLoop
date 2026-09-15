namespace IntelliLoop.Web.Services.Background
{
    public class BackgroundTaskWorker : BackgroundService 
    {
        private readonly BackgroundTaskQueue _queue;

        public BackgroundTaskWorker(BackgroundTaskQueue queue)
        {
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var work = await _queue.DequeueAsync();

                try
                {
                    await work();
                }
                catch (Exception ex)
                {
                    //TODO: add logging here
                    Console.WriteLine($"Background job failed: {ex}");
                }
            }
        }
    }
}
