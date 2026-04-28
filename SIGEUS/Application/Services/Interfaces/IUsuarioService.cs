using SIGEUS.Application.DTOs;
using SIGEUS.Domain.Entities;

namespace SIGEUS.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<RetornoUsuarioDto> CadastrarAsync(CadastroUsuarioDto dto);
    Task<RetornoUsuarioDto?> BuscarPorEmailAsync(string email);
    Task<RetornoUsuarioDto?> BuscarPorIdAsync(Guid id);
    Task<IEnumerable<DocumentoDto>?> ObterDocumentosPorUsuarioAsync(Guid usuarioId);
    Task AlterarUsuarioSeguroAsync(Guid id, string emailInformado, string senhaPura,
        CadastroUsuarioDto novosDados);
    Task<RetornoUsuarioDto?> InativarUsuarioSeguroAsync(Guid id, string emailInformado, string senhaPura);
    Task<RetornoUsuarioDto?> AtivarUsuarioSeguroAsync(string emailInformado, string senhaPura);
    Task<IEnumerable<RetornoUsuarioDto>?> ObterTodosAsync();
}