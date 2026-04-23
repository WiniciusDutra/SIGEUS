using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEUS.Infra.Data;

namespace SIGEUS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _context.Usuario.ToListAsync();
        return Ok(lista);
    }
}