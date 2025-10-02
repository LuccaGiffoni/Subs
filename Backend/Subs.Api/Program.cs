using Microsoft.EntityFrameworkCore;
using Npgsql;
using Subs.Core.Data;
using Subs.Core.Services.Bus;
using Subs.Core.Services.Entities;
using Subs.Domain.DTOs.Mapping;
using Subs.Domain.Interfaces.Bus;
using Subs.Domain.Interfaces.Entities;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models.Messages;
using System.Text.Json.Serialization;

NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHealthChecks();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddDbContext<SubsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBusService<SubscriptionMessage>, SubscriptionBusService>();
builder.Services.AddScoped<IBusService<ClientMessage>, ClientBusService>();

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<ISubscriptionEventHistoryService, SubscriptionEventHistoryService>();
builder.Services.AddScoped<IClientEventHistoryService, ClientEventHistoryService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SubsDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowLocalhost3000");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();