using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace SIGEUS.Filters;

public class SigeusExceptionFilter(ILogger<SigeusExceptionFilter> logger) : IExceptionFilter
{
    private readonly ILogger<SigeusExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, $"Erro detectado no Exception Filter: {context.Exception.Message}");
        
        var (statusCode, mensagem) = context.Exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, context.Exception.Message),
            InvalidOperationException => (HttpStatusCode.NotFound, context.Exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor. Tente novamente mais tarde.")
        };
        
        var resultado = new ObjectResult(new 
        { 
            msg = mensagem,
        })
        {
            StatusCode = (int)statusCode
        };

        context.Result = resultado;
        context.ExceptionHandled = true; 
    }
}