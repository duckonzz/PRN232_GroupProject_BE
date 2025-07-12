using FluentValidation;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Repositories.Repositories;
using GenderHealthCare.Service;
using GenderHealthCare.Services.Infrastructure;
using GenderHealthCare.Services.Infrastructure.Emailing;
using GenderHealthCare.Services.Services;
using GenderHealthCare.Services.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<ICycleTrackingService, CycleTrackingService>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAvailableSlot, AvailableSlotService>();
            services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
            services.AddScoped<IConsultationsService, ConsultationService>();
            services.AddScoped<IConsultantScheduleService, ConsultantScheduleService>();
            services.AddScoped<ITestSlotService, TestSlotService>();
            services.AddScoped<ITestBookingService, TestBookingService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IHealthTestService, HealthTestService>();
            services.AddScoped<IHealthTestScheduleService, HealthTestScheduleService>();
            services.AddScoped<IQAThreadService, QAThreadService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<JwtTokenGenerator>();
        }
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IConsultantRepository, ConsultantRepository>();
            services.AddScoped<IConsultantScheduleRepository, ConsultantScheduleRepository>();
            services.AddScoped<ITestSlotRepository, TestSlotRepository>();
            services.AddScoped<ITestBookingRepository, TestBookingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IQAThreadRepository, QAThreadRepository>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UserRegistrationRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();

            services.AddValidatorsFromAssemblyContaining<CycleTrackingRequestValidator>();

            services.AddValidatorsFromAssemblyContaining<UpdateConsultationResultRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<ConsultationRequestValidator>();
        }
    }
}
