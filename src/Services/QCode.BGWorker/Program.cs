using QCode.Application;
using QCode.Application.Common.Options;
using QCode.BGWorker;
using Serilog;
using System.Diagnostics;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(opt =>
    {
        opt.ServiceName = "QCodeChallangeService";
    })
    .UseSerilog((ctx, cl) =>
    {
        cl.ReadFrom.Configuration(ctx.Configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices(context.Configuration);
        services.Configure<BGWorkerOptions>(context.Configuration.GetSection(nameof(BGWorkerOptions)));
        services.AddHostedService<Producer>();
        services.AddHostedService<Consumer>();
        services.AddHostedService<ErrorConsumer>();
    })
    .Build();

Serilog.Debugging.SelfLog.Enable(msg => {
    Debug.WriteLine(msg);
});

await host.RunAsync();




