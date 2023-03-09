using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QCode.Application.Common.Behaviors;
using QCode.Application.Common.Options;
using QCode.Application.Interfaces;
using QCode.Application.Services;
using Services;
using System.IO.Abstractions;
using System.Reflection;

namespace QCode.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });

            services.AddScoped<CSVReportCreator>();
            services.AddScoped<TxtReportCreator>();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<IPowerService, PowerService>();
            services.AddScoped<IReportCreatorFactory, ReportCreatorFactory>();
            services.AddSingleton<IDateTime, UKLocalTimeService>();
            services.AddSingleton<IExtractAttemptManager, InMemoryExtractManager>(p =>
            {
                return new InMemoryExtractManager(
                    p.GetRequiredService<ILogger<InMemoryExtractManager>>(),
                    default,
                    default);
            });

            services.Configure<FileReportOptions>(configuration.GetSection(nameof(FileReportOptions)));
            services.Configure<PowerServiceOptions>(configuration.GetSection(nameof(PowerServiceOptions)));
            services.Configure<PerformanceBehaviorOptions>(configuration.GetSection(nameof(PerformanceBehaviorOptions)));

            return services;
        }
    }
}
