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
    
    public async Task<Usuario?> BuscarPorIdAsync(Guid id)
    {
        return await _repository.ObterPorIdAsync(id);
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        if (!email.Contains("@")) throw new ArgumentException("Formato de e-mail inválido.");
        return await _repository.ObterPorEmailAsync(email);
    }
    
    public async Task AlterarUsuarioSeguroAsync(Guid id, string emailInformado, string senhaPura, CadastroUsuarioDto novosDados)
    {
        var usuario = await _repository.ObterPorIdAsync(id);

        if (usuario == null || !usuario.Ativo)
            throw new InvalidOperationException("Usuário não encontrado ou inativo.");
        
        if (usuario.Email != emailInformado)
            throw new UnauthorizedAccessException("E-mail de confirmação incorreto.");
        
        bool senhaValida = BCrypt.Net.BCrypt.Verify(senhaPura, usuario.SenhaHash);

        if (!senhaValida)
            throw new UnauthorizedAccessException("Senha incorreta. Operação não autorizada.");
        
        usuario.AlterarDados(novosDados.Nome, novosDados.Cargo, id);
    
        await _repository.SalvarAlteracoesAsync();
    }
    
    public async Task InativarUsuarioSeguroAsync(Guid id, string emailInformado, string senhaPura)
    {
        var usuario = await _repository.ObterPorIdAsync(id);

        if (usuario == null || !usuario.Ativo)
            throw new InvalidOperationException("Usuário não encontrado ou inativo.");
        
        if (usuario.Email != emailInformado)
            throw new UnauthorizedAccessException("E-mail de confirmação incorreto.");
        
        bool senhaValida = BCrypt.Net.BCrypt.Verify(senhaPura, usuario.SenhaHash);

        if (!senhaValida)
            throw new UnauthorizedAccessException("Senha incorreta. Operação não autorizada.");
        
        usuario.Desativar();
    
        await _repository.SalvarAlteracoesAsync();
    }
}