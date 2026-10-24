using IntelliLoop.Core.Contracts.Repositories;
using IntelliLoop.Core.Contracts.Services;
using IntelliLoop.Core.Services;
using IntelliLoop.Infrastructure.Data.Repositories;
using IntelliLoop.Infrastructure.Services;

namespace IntelliLoop.Web.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPromptService, PromptService>();

            services.AddScoped<ILectureService, LectureService>();

            services.AddScoped<ILectureProcessingService, LectureProcessingService>();

            services.AddScoped<ILectureProcessingRepository, LectureProcessingRepository>();

            services.AddScoped<ILectureRepository, LectureRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFileStorage, FileStorage>();

            services.AddScoped<ITranscriptionService, TranscriptionService>();

            services.AddHostedService<LectureWorker>();

            services.AddSingleton<IWhisperModelService, WhisperModelService>();

            return services;
        }
    }
}
