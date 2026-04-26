using SIGEUS.Application.DTOs;
using SIGEUS.Application.Mappers;
using SIGEUS.Application.Services.Interfaces;
using SIGEUS.Domain.Entities;
using SIGEUS.Domain.Interfaces;

namespace SIGEUS.Application.Services;

public class UsuarioService(IUsuarioRepository repository): IUsuarioService
{
    private readonly IUsuarioRepository _repository = repository;
    
    public async Task<RetornoUsuarioDto> CadastrarAsync(CadastroUsuarioDto dto)
    {
        var existente = await _repository.ObterPorEmailAsync(dto.Email);
        if (existente != null)
            throw new InvalidOperationException("E-mail já cadastrado.");
        
        var novoUsuario = new Usuario(dto.Nome, dto.Email, dto.Senha, dto.Cargo);

        await _repository.AdicionarAsync(novoUsuario);
        await _repository.SalvarAlteracoesAsync();

        return novoUsuario.ToRetornoUsuarioDto();
    }
    
    public async Task<RetornoUsuarioDto?> BuscarPorIdAsync(Guid id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        
        if (usuario == null || !usuario.Ativo) return new RetornoUsuarioDto();
        
        return usuario.ToRetornoUsuarioDto();
    }

    public async Task<RetornoUsuarioDto?> BuscarPorEmailAsync(string email)
    {
        if (!email.Contains("@")) throw new ArgumentException("Formato de e-mail inválido.");
        
        var usuario = await _repository.ObterPorEmailAsync(email);
        
        if (usuario == null || !usuario.Ativo) return new RetornoUsuarioDto();
        
        return usuario.ToRetornoUsuarioDto();
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
    
    public async Task<RetornoUsuarioDto?> InativarUsuarioSeguroAsync(Guid id, string emailInformado, string senhaPura)
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

        return usuario.ToRetornoUsuarioDto();
    }
    
    public async Task<RetornoUsuarioDto?> AtivarUsuarioSeguroAsync(string emailInformado, string senhaPura)
    {
        var usuario = await _repository.ObterPorEmailAsync(emailInformado);

        if (usuario == null || usuario.Ativo)
            throw new InvalidOperationException("Usuário não encontrado ou já ativo.");
        
        bool senhaValida = BCrypt.Net.BCrypt.Verify(senhaPura, usuario.SenhaHash);

        if (!senhaValida)
            throw new UnauthorizedAccessException("Senha incorreta. Operação não autorizada.");
        
        usuario.Ativar();
    
        await _repository.SalvarAlteracoesAsync();
        
        return usuario.ToRetornoUsuarioDto();
    }
    
    public async Task<IEnumerable<RetornoUsuarioDto>?> ObterTodosAsync()
    {
        var usuarios = await _repository.ObterTodosAsync();
        
        if (usuarios.Count() == 0 ) return [];
        
        return usuarios.Where(u => u.Ativo).Select(u => u.ToRetornoUsuarioDto());
    }
}