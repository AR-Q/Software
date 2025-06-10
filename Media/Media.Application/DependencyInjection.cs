using Media.Application.MediaService.Delete;
using Media.Application.MediaService.Delete.Intefrace;
using Media.Application.MediaService.Get;
using Media.Application.MediaService.Get.Interface;
using Media.Application.MediaService.Save;
using Media.Application.MediaService.Save.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISave, Save>();
            services.AddScoped<IGet, Get>();
            services.AddScoped<IDelete, Delete>();
            return services;
        }
    }
}