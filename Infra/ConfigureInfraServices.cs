using Application.Interfaces;
using Infra.Data;
using Infra.NoSql;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra
{
    public static class ConfigureInfraServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
            });

            services.AddSingleton<AuditDbContext>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
