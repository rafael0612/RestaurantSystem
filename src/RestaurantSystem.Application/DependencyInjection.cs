using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Application.Services;
using RestaurantSystem.Application.Validators;

namespace RestaurantSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Services / Use Cases
            services.AddScoped<IMeseroService, MeseroService>();
            services.AddScoped<ICocinaService, CocinaService>();
            services.AddScoped<ICajaService, CajaService>();

            // Validators
            services.AddValidatorsFromAssemblyContaining<RegistrarPagoRequestValidator>();

            return services;
        }
    }
}
