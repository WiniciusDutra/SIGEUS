using Microsoft.EntityFrameworkCore;
using SIGEUS.Domain.Entities;
using SIGEUS.Domain.Interfaces;
using SIGEUS.Infra.Data;

namespace SIGEUS.Infra.Repositories;

public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
{
    private readonly AppDbContext _context = context;
    
    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuario
            .Include(u => u.Documentos)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuario
            .Include(u => u.Documentos)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<Usuario>> ObterTodosAsync() 
        => await _context.Usuario.
            Include(u => u.Documentos).
            Where(u => u.Ativo).ToListAsync();

    public async Task AdicionarAsync(Usuario usuario) 
        => await _context.Usuario.AddAsync(usuario);
    
    public async Task AtualizarAsync(Usuario usuario) 
        => _context.Usuario.Update(usuario);

    public async Task SalvarAlteracoesAsync()   
        => await _context.SaveChangesAsync();
    
}