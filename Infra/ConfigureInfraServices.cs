using Application.Interfaces;
using Infra.Data;
using Infra.Identity;
using Infra.NoSql;
using Infra.Repositories;
using Infra.Services;
using Microsoft.AspNetCore.Identity;
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
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();

            // Registrar serviços de autenticação
            services.AddScoped<IAuthService, AuthService>();

            // Registrar serviços de importação e exportação
            services.AddHttpClient<IImportService, ImportService>();
            services.AddScoped<IExportService, ExportService>();

            // Usar AddIdentityCore em vez de AddIdentity para APIs (sem autenticação por cookie)
            services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;

                opt.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
