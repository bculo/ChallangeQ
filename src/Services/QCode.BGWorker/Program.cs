using QCode.Application;
using QCode.BGWorker;
using QCode.BGWorker.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices(context.Configuration);
        services.Configure<BGWorkerOptions>(context.Configuration.GetSection(nameof(BGWorkerOptions)));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
