using IntelliLoop.Core.Interfaces;
using IntelliLoop.Core.Repository;
using IntelliLoop.Web.Repositories;
using IntelliLoop.Web.Services;

namespace IntelliLoop.Web.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPromptService, PromptService>();

            services.AddScoped<ILectureGenerationService, LectureGenerationService>();

            services.AddScoped<LectureProcessingService>();

            services.AddScoped<ILectureProcessingRepository, LectureProcessingRepository>();

            services.AddScoped<ILectureRepository, LectureRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFileStorage, FileStorage>();

            services.AddHostedService<LectureWorker>();

            return services;
        }
    }
}
