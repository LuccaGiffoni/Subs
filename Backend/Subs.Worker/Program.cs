using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Core.Services.Entities;
using Subs.Core.Services.Worker;
using Subs.Domain.Interfaces.History;
using Subs.Worker.Workers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<SubsDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<SubscriptionWorkerService>();
        services.AddHostedService<SubscriptionWorker>();

        services.AddScoped<ClientWorkerService>();
        services.AddHostedService<ClientWorker>();

        services.AddScoped<ISubscriptionEventHistoryService, SubscriptionEventHistoryService>();
        services.AddScoped<IClientEventHistoryService, ClientEventHistoryService>();
    })
    .Build();

await host.RunAsync();