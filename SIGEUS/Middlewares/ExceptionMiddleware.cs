using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace SIGEUS.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
            InvalidOperationException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;
        
        var resultado = new ObjectResult(new 
        { 
            msg = exception.Message,
        })
        {
            StatusCode = statusCode
        };
        var json = JsonSerializer.Serialize(resultado);

        return context.Response.WriteAsync(json);
    }
    
}