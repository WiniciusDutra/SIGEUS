using SIGEUS.Application.DTOs;
using SIGEUS.Domain.Entities;

namespace SIGEUS.Application.Mappers;

public static class UsuarioMapper
{
    public static Usuario ToUsuario(this CadastroUsuarioDto dto)
    {
        var usuario = new Usuario(dto.Nome, dto.Email, dto.Senha, dto.Cargo);
        foreach (var doc in dto.Documentos)
        {
            usuario.Documentos.Add(new Documento(doc.Nome, doc.Extensao, doc.Tamanho, usuario.Id));
        }
        return usuario;
    }
    
    public static CadastroUsuarioDto ToCadastroUsuarioDto(this Usuario entity)
    {
        return new CadastroUsuarioDto
        {
            Nome = entity.Nome,
            Email = entity.Email,
            Cargo = entity.Cargo,
        };
    }
    
    public static RetornoUsuarioDto ToRetornoUsuarioDto(this Usuario entity)
    {
        return new RetornoUsuarioDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Email = entity.Email,
            Cargo = entity.Cargo,
            CriadoEm = entity.CriadoEm,
            AtualizadoEm = entity.AtualizadoEm,
            Documentos = entity.Documentos.Select(d => new DocumentoDto 
            { 
                Nome = d.Nome, 
                Extensao = d.Extensao, 
                Tamanho = d.Tamanho 
            }).ToList()
        };
    }
    

}