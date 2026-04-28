using System.ComponentModel.DataAnnotations;

namespace SIGEUS.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? Cargo { get; private set; }
    
    public ICollection<Documento> Documentos { get; private set; } = new List<Documento>();
    
    private Usuario() { }
    
    public Usuario(string nome, string email, string senhaPura, string? cargo)
    {
        Validar(nome, email);
        
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        Ativo = true;
        CriadoEm = DateTime.UtcNow;
        Cargo = cargo;
        
        SetSenha(senhaPura);
    }

    public void Validar(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) throw new ArgumentException("E-mail inválido.");
    }

    public void SetSenha(string senhaPura)
    {
        if (string.IsNullOrWhiteSpace(senhaPura) || senhaPura.Length < 6)
            throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
        
        this.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senhaPura);
        RegistrarAtualizacao();
    }

    public void AlterarDados(string nome, string? cargo, Guid solicitanteId)
    {
        if (!TemPermissaoParaAlterar(solicitanteId))
            throw new UnauthorizedAccessException("Apenas o próprio usuário pode alterar seus dados.");

        Nome = nome;
        Cargo = cargo;
        RegistrarAtualizacao();
    }

    public bool EstaAtivo() => Ativo;

    public void Ativar() { Ativo = true; RegistrarAtualizacao(); }
    
    public void Desativar() { Ativo = false; RegistrarAtualizacao(); }

    public bool TemPermissaoParaAlterar(Guid solicitanteId) => this.Id == solicitanteId;

    private void RegistrarAtualizacao()
    {
        AtualizadoEm = DateTime.UtcNow;
    }
}