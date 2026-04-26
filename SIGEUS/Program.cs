using Microsoft.EntityFrameworkCore;
using SIGEUS.Application.Services;
using SIGEUS.Application.Services.Interfaces;
using SIGEUS.Domain.Interfaces;
using SIGEUS.Infra.Data;
using SIGEUS.Infra.Repositories;
using Serilog;
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
    
            // Adiciona o exception filter individualmente nos endpoints
            builder.Services.AddControllers();
            builder.Services.AddScoped<Filters.SigeusExceptionFilter>();
            builder.Services.AddScoped<Filters.ValidarEmailAttribute>();
            
            // Adiciona o exception filter globalmente
            //builder.Services.AddControllers(options =>
            //{
            //    options.Filters.Add<Filters.SigeusExceptionFilter>();
            //});
            
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