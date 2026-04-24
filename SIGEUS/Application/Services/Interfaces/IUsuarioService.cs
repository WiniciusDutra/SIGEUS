using SIGEUS.Application.DTOs;
using SIGEUS.Domain.Entities;

namespace SIGEUS.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> CadastrarAsync(CadastroUsuarioDto dto);
}