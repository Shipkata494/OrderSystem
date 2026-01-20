using System.Text;
using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

using StackExchange.Redis;

using OrderPlatform.Domain.Entities;

using OrderPlatform.Infrastructure.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"));
        });


        var jwt = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "orderuser";
        var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "orderpass";

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitHost, h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });
            });
        }); 
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderPlatform.Api", Version = "v1" });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter: Bearer {your JWT token}"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });


        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}