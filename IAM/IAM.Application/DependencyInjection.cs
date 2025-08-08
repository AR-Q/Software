using IAM.Application.AuthenticationService;
using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserLoginService, UserLoginService>();
            services.AddScoped<IAdminLoginService, AdminLoginService>();
            services.AddScoped<IUserRegisterService, UserRegisterService>();
            services.AddScoped<IUserUpdateService, UserUpdateService>();
            services.AddScoped<IUserVerifyService, UserVerifyService>();
            services.AddSingleton<ITokenCheck, TokenCheck>();
            return services;
        }
    }
}
