using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ConfigureApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar serviços de aplicação
            services.AddScoped<IProductApplicationService, ProductAplicationService>();
            services.AddScoped<ITagApplicationService, TagApplicationService>();
            services.AddScoped<ICategoryApplicationService, CategoryApplicationService>();

            return services;
        }
    }
}
