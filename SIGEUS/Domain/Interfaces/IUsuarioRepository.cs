using SIGEUS.Domain.Entities;

namespace SIGEUS.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ObterTodosAsync();
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task SalvarAlteracoesAsync();
}