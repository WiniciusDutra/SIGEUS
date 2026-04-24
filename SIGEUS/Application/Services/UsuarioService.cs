using SIGEUS.Application.DTOs;
using SIGEUS.Application.Services.Interfaces;
using SIGEUS.Domain.Entities;
using SIGEUS.Domain.Interfaces;

namespace SIGEUS.Application.Services;

public class UsuarioService: IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository) => _repository = repository;

    public async Task<Usuario> CadastrarAsync(CadastroUsuarioDto dto)
    {
        var existente = await _repository.ObterPorEmailAsync(dto.Email);
        if (existente != null)
            throw new InvalidOperationException("E-mail já cadastrado.");
        
        var novoUsuario = new Usuario(dto.Nome, dto.Email, dto.Senha, dto.Cargo);

        await _repository.AdicionarAsync(novoUsuario);
        await _repository.SalvarAlteracoesAsync();

        return novoUsuario;
    }
}