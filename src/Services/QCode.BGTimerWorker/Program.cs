using QCode.Application;
using QCode.Application.Common.Options;
using QCode.BGTimerWorker;
using Serilog;
using System.Diagnostics;

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
