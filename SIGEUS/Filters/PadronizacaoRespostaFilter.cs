using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace SIGEUS.Filters;

public class PadronizacaoRespostaFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var executedContext = await next();
        
        stopwatch.Stop();
        
        if (executedContext.Result is ObjectResult objectResult)
        {
            var dadosOriginais = objectResult.Value;

            var resultadoFinal = new
            {
                dados_resposta = dadosOriginais,
                timestamp_resposta = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                tempo_da_resposta = $"{stopwatch.ElapsedMilliseconds} ms"
            };

            objectResult.Value = resultadoFinal;
        }
    }
}