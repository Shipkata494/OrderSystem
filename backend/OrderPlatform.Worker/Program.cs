using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderPlatform.Infrastructure.Data;
using OrderPlatform.Worker.Consumers;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
        });

        cfg.ReceiveEndpoint("orderplatform-order-created", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var host = builder.Build();
await host.RunAsync();
