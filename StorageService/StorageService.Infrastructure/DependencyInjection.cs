using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Interfaces;
using StorageService.Infrastructure.Services;

namespace StorageService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<ICloudStorageService, MinioStorageService>();
            return services;
        }
    }
}