using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Presentation.Extensions
{
    public static class ServiceExtensions
    {
        //public static void ConfigureCors(this IServiceCollection services) =>
        //  services.AddCors(options =>
        //  {
        //      options.AddPolicy("CorsPolicy", builder =>
        //        builder.AllowAnyOrigin()
        //        .AllowAnyMethod()
        //        .AllowAnyHeader()
        //        .WithExposedHeaders("X-Pagination"));
        //  });
        public static void ConfigureDatabaseContext(this IServiceCollection services, IConfiguration configuration) => 
            services.AddDbContext<RepositoryContext>(opts => opts.UseSqlite(configuration.GetConnectionString("sqlConnection")));

        //  STAGE 2
        public static void ConfigureRepositoryManager(this IServiceCollection services) => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) => services.AddScoped<IServiceManager, ServiceManager>();

        //public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration) =>
        //   services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //   .AddJwtBearer(t =>
        //   {
        //       t.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        //       {
        //           ValidateIssuerSigningKey = true,
        //           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"])),
        //           ValidateIssuer = true,
        //           ValidIssuer = configuration["jwt:audience"],
        //           ValidAudience = configuration["jwt:audience"],
        //           ValidateAudience = true,
        //           ValidateLifetime = true,
        //           ClockSkew = TimeSpan.FromMinutes(5)
        //       };
        //   });

        //public static void ConfigureAuthorization(this IServiceCollection services) =>
        //    services.AddAuthorization(opt =>
        //    {
        //        opt.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        //        .RequireAuthenticatedUser()
        //        .Build();
        //    });

        //public static void ConfigureTokenManager(this IServiceCollection services) =>
        //   services.AddScoped<ITokenManager, TokenManager>();
    }
}
