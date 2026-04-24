using SIGEUS.Application.DTOs;
using SIGEUS.Domain.Entities;

namespace SIGEUS.Application.Mappers;

public static class UsuarioMapper
{
    public static Usuario ToUsuario(this CadastroUsuarioDto dto)
    {
        return new Usuario(
            dto.Nome,
            dto.Email, 
            dto.Senha,
            dto.Cargo
            );
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
}