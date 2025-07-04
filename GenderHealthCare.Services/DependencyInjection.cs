using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Repositories.Repositories;
using System.Reflection;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Services.Validators;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Services.Services;
using GenderHealthCare.Services.Infrastructure;


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
            services.AddScoped<IConsultantService, ConsultantService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<JwtTokenGenerator>();
            services.AddScoped<IConsultantScheduleService, ConsultantScheduleService>();
            services.AddScoped<ITestSlotService, TestSlotService>();
            services.AddScoped<ITestBookingService, TestBookingService>();

        }
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IConsultantRepository, ConsultantRepository>();
            services.AddScoped<IConsultantScheduleRepository, ConsultantScheduleRepository>();
            services.AddScoped<ITestSlotRepository, TestSlotRepository>();
            services.AddScoped<ITestBookingRepository, TestBookingRepository>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UserRegistrationRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
        }
    }
}
