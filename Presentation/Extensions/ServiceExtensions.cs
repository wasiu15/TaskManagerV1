using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;
using TaskManager.Infrastructure.Utilities;

namespace TaskManager.Presentation.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDatabaseContext(this IServiceCollection services, IConfiguration configuration) => 
            services.AddDbContext<RepositoryContext>(opts => opts.UseSqlite(configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) => services.AddScoped<IServiceManager, ServiceManager>();
        
        public static void ConfigureServiceProvider(this IServiceCollection services) => services.AddScoped<IServiceProvider, ServiceProvider>();

        public static void ConfigureTokenManager(this IServiceCollection services) => services.AddScoped<ITokenManager, TokenManager>();

        public static void ConfigureHttpclient(this IServiceCollection services) => services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration) =>
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(t =>
            {
                t.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["jwt:audience"],
                    ValidAudience = configuration["jwt:audience"],
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });

        public static void ConfigureAuthorization(this IServiceCollection services) =>
            services.AddAuthorization(opt =>
            {
                opt.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });
    }
}
