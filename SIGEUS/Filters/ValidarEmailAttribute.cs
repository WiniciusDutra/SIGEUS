using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SIGEUS.Application.DTOs;

namespace SIGEUS.Filters;

public class ValidarEmailAttribute : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var emailStr = context.ActionArguments.FirstOrDefault(a => 
            a.Key.Equals("email", StringComparison.OrdinalIgnoreCase)).Value as string;
        
        if (string.IsNullOrEmpty(emailStr))
        {
            var dto = context.ActionArguments.Values.FirstOrDefault(v => v is CadastroUsuarioDto) as CadastroUsuarioDto;
            emailStr = dto?.Email;
        }

        if (!string.IsNullOrEmpty(emailStr) && !emailStr.Contains('@'))
        {
            context.Result = new BadRequestObjectResult(new { msg = "email válido deve conter um @." });
            return;
        }

        await next();
    }
}