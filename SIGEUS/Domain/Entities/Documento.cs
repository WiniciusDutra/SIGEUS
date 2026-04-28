namespace SIGEUS.Domain.Entities;

public class Documento
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Extensao { get; private set; }
    public float Tamanho { get; private set; }
    
    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    private Documento(){}

    public Documento(string nome, string extensao, float tamanho, Guid usuarioId)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Extensao = extensao;
        Tamanho = tamanho;
        UsuarioId = usuarioId;
    }
}