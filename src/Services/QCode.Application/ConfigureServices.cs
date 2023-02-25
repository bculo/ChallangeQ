using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using Services;
using System.Reflection;

namespace QCode.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IPowerService, PowerService>();
            services.AddScoped<IReportCreatorFactory, ReportCreatorFactory>();
            services.AddScoped<CSVReportCreator>();
            services.AddSingleton<IDateTime, UKLocalTimeService>();

            return services;
        }
    }
}
