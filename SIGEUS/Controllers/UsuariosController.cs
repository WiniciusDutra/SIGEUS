using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEUS.Application.DTOs;
using SIGEUS.Application.Mappers;
using SIGEUS.Application.Services;
using SIGEUS.Application.Services.Interfaces;
using SIGEUS.Infra.Data;

namespace SIGEUS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IUsuarioService usuarioService): ControllerBase
{
    private readonly IUsuarioService _usuarioService =  usuarioService;
    
    [HttpPost("usuario")]
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
    
    [HttpGet("usuario/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      
        var usuario = await _usuarioService.BuscarPorIdAsync(id);

        if (usuario == null)
            return NotFound(new { msg = "Usuário não encontrado." });
        
        var usuarioRetorno = usuario.ToRetornoUsuarioDto();

        return Ok(usuarioRetorno);
    }
    
    [HttpGet("buscar-usuario-por-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        try 
        {
            var usuario = await _usuarioService.BuscarPorEmailAsync(email);

            if (usuario == null)
                return NotFound(new { msg = $"Nenhum usuário com o e-mail {email} foi encontrado." });

            var usuarioRetorno = usuario.ToRetornoUsuarioDto();

            return Ok(usuarioRetorno);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
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
            await _usuarioService.InativarUsuarioSeguroAsync(id, confirmacao.Email, confirmacao.Senha);
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
}
