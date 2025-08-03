using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Code;
using IAM.Application.Common.Hash;
using IAM.Application.Common.Repositories;
using IAM.Application.Common.Tokens;
using IAM.Infrastructure.Code;
using IAM.Infrastructure.context;
using IAM.Infrastructure.DistrebutedCaching;
using IAM.Infrastructure.Email;
using IAM.Infrastructure.Hash;
using IAM.Infrastructure.Repositories;
using IAM.Infrastructure.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<SQLServerDataContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddScoped<ICodeGenerator, CodeGenerator>();
            services.AddScoped<IHasher, Hasher>();
            services.AddScoped<ICachingContext, RedisContext>();
            services.AddSingleton<IEmailService, EmailService>();
            return services;
        }
    }
}
