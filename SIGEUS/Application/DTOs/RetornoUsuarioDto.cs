namespace SIGEUS.Application.DTOs;

public class RetornoUsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string? Cargo { get; set; }
}