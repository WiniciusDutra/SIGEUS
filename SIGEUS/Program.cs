using Microsoft.EntityFrameworkCore;
using SIGEUS.Application.Services;
using SIGEUS.Application.Services.Interfaces;
using SIGEUS.Domain.Interfaces;
using SIGEUS.Infra.Data;
using SIGEUS.Infra.Repositories;
using Serilog;
using Serilog.Formatting.Json;
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

            // Registro do Repository
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Registro da Service
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "API v1"); });
            }

            app.UseHttpsRedirection();

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