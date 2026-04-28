using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SIGEUS.Application.Services;
using SIGEUS.Application.Services.Interfaces;
using SIGEUS.Domain.Interfaces;
using SIGEUS.Infra.Data;
using SIGEUS.Infra.Repositories;
using Serilog;
using SIGEUS.Converters;
using SIGEUS.Filters;
using SIGEUS.Middlewares;

namespace SIGEUS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        try
        {
            Log.Information("Iniciando a API SIGEUS...");
            
            builder.Host.UseSerilog();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));
            
            // //Config global de serialização json
            // builder.Services.ConfigureHttpJsonOptions(options =>
            // {
            //     // camelCase nas propriedades
            //     options.SerializerOptions.PropertyNamingPolicy =
            //         JsonNamingPolicy.SnakeCaseLower;
            //
            //     // ignorar propriedades nulas
            //     options.SerializerOptions.DefaultIgnoreCondition =
            //         JsonIgnoreCondition.WhenWritingNull;
            //
            //     // serializar enums como string
            //     options.SerializerOptions.Converters.Add(
            //         new JsonStringEnumConverter());
            //
            //     // aceitar números como strings e vice-versa
            //     options.SerializerOptions.NumberHandling =
            //         JsonNumberHandling.AllowReadingFromString;
            // });

            // Registro do Repository
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Registro da Service
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
    
            // Adiciona o exception filter individualmente nos endpoints
            //builder.Services.AddControllers();
            //builder.Services.AddScoped<Filters.SigeusExceptionFilter>();
            //builder.Services.AddScoped<Filters.ValidarEmailAttribute>();
            
            // Adiciona filtros globalmente
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<PadronizacaoRespostaFilter>();
                
                options.Filters.Add<SigeusExceptionFilter>();
            })
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //configurando CORS
            var frontDoBruno = builder.Configuration["FrontendSettings:FrontDoBruno"];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "FrontDoBrunoPolicy",
                    policy =>
                    {
                        policy.WithOrigins(frontDoBruno!)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
            var app = builder.Build();
            
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var addressFeature = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
                    .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();

                if (addressFeature != null)
                {
                    foreach (var address in addressFeature.Addresses)
                    {
                        Log.Information("Aplicação rodando em {Address}", address);
                    }
                }
            });
            
            app.UseHttpsRedirection();
            
            app.UseCors("FrontDoBrunoPolicy");
            
            //Registrando middleware de tratamento global de exceções
            app.UseMiddleware<ExceptionMiddleware>();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "API v1"); });
            }
            
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "A aplicação falhou ao iniciar.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}