using SIGEUS.Application.DTOs;
using SIGEUS.Domain.Entities;

namespace SIGEUS.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> CadastrarAsync(CadastroUsuarioDto dto);
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<Usuario?> BuscarPorIdAsync(Guid id);
}