using IntelliLoop.Core.Entities;
using System.Threading.Channels;

namespace IntelliLoop.Web.Services.Background
{
    public class LectureWorker : BackgroundService
    {
        private readonly Channel<Guid> _channel;
        private readonly ILogger<LectureWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;


        public LectureWorker(Channel<Guid> channel,
            ILogger<LectureWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _channel = channel;
            _logger = logger;
            _scopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation("Lecture generation worker started.");

            while (await _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                while (_channel.Reader.TryRead(out var jobId))
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var lectureProcessingService = scope.ServiceProvider.GetRequiredService<LectureProcessingService>();
                        await lectureProcessingService.ProcessLectureAsync(jobId);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing a lecture generation job.");
                    }
                }
            }
        }
    }
}
