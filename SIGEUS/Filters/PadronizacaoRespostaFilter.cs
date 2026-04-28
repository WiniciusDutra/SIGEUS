using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace SIGEUS.Filters;

public class PadronizacaoRespostaFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        if (context.Result is ObjectResult objectResult)
        {
            var dadosOriginais = objectResult.Value;
            
            stopwatch.Stop(); 

            var resultadoFinal = new
            {
                dados_resposta = dadosOriginais,
                timestamp_resposta = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                tempo_da_resposta = $"{stopwatch.ElapsedMilliseconds} ms"
            };

            objectResult.Value = resultadoFinal;
        }

        await next(); 
    }
}