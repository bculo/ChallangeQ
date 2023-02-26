using QCode.Application;
using QCode.BGWorker;
using QCode.BGWorker.Options;
using System.Diagnostics;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, cl) =>
    {
        cl.ReadFrom.Configuration(ctx.Configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices(context.Configuration);
        services.Configure<BGWorkerOptions>(context.Configuration.GetSection(nameof(BGWorkerOptions)));
        services.AddHostedService<Worker>();
    })
    .Build();

Serilog.Debugging.SelfLog.Enable(msg => {
    Debug.WriteLine(msg);
});

await host.RunAsync();




