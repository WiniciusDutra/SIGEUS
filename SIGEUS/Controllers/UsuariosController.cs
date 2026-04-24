using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEUS.Application.DTOs;
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
      
        return Ok(); 
    }
}