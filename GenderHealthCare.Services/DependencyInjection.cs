using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Repositories.Repositories;
using System.Reflection;


namespace GenderHealthCare.Services
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(configuration);
            services.AddRepository();
            services.AddValidators();
        }
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {

        }
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
        }
    }
}
