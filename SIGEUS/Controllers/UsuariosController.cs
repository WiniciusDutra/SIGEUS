using Microsoft.AspNetCore.Mvc;
using SIGEUS.Application.DTOs;
using SIGEUS.Application.Services.Interfaces;


namespace SIGEUS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioService usuarioService): ControllerBase
{
    private readonly IUsuarioService _usuarioService =  usuarioService;
    
    [HttpPost]
    public async Task<IActionResult> Usuario([FromBody] CadastroUsuarioDto dto)
    {
        try
        {
            var usuario = await _usuarioService.CadastrarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { msg = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      
        var usuario = await _usuarioService.BuscarPorIdAsync(id);

        if (usuario == null)
            return NotFound(new { msg = "Usuário não encontrado." });
        
        var usuarioRetorno = usuario;

        return Ok(usuarioRetorno);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            var usuario = await _usuarioService.BuscarPorEmailAsync(email);
        
            if (usuario == null)
                return NotFound(new { msg = $"Nenhum usuário com o e-mail {email} foi encontrado." });

            return Ok(usuario);
        }
        
        var todos = await _usuarioService.ObterTodosAsync();
        return Ok(todos);
    }
    

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CadastroUsuarioDto dados, [FromQuery] string email, [FromHeader] string senha)
    {
        try
        {
            await _usuarioService.AlterarUsuarioSeguroAsync(id, email, senha, dados);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { msg = ex.Message }); 
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { msg = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Inativar(Guid id, [FromBody] ConfirmacaoOperacaoDto confirmacao)
    {
        try
        {
            var usuario = await _usuarioService.InativarUsuarioSeguroAsync(id, confirmacao.Email, confirmacao.Senha);
            return Ok(new { msg = $"Usuário {usuario.Nome} inativado com sucesso!" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { msg = ex.Message }); 
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { msg = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { msg = ex.Message });
        }
    }
    
    [HttpPatch("ativar")]
    public async Task<IActionResult> Ativar([FromBody] ConfirmacaoOperacaoDto confirmacao)
    {
        try
        {
            var usuario = await _usuarioService.AtivarUsuarioSeguroAsync(confirmacao.Email, confirmacao.Senha);
        
            return Ok(new { msg = $"Usuário {usuario.Nome} reativado com sucesso!" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { msg = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { msg = "Erro interno ao processar a ativação." });
        }
    }
}
