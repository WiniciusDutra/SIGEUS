# 🚀 SIGEUS
> Sistema de Gestão de Usuários

## 💻 Sobre o Projeto
Projeto com finalidade educacional para demosntração de API REST com algumas funcionaldiade aplicadas

## 🛠️ Tecnologias Utilizadas
- .NET Core 10;
- Persistência em banco de dados SqLite;
- Criação do bd com Entity Framework Core e Migrations;
- Registro de logs com Serilog;
- Validação via Action Filters e Middlewares customizados;

## 🛠️ dependências
- BCrypt.Net-Next                            4.1.0 
- Microsoft.AspNetCore.OpenApi              10.0.6
- Microsoft.EntityFrameworkCore.Design      10.0.7
- Microsoft.EntityFrameworkCore.Sqlite      10.0.7
- Serilog.AspNetCore                        10.0.0
- Serilog.Settings.Configuration            10.0.0
- Serilog.Sinks.File                         7.0.0 
- Swashbuckle.AspNetCore.SwaggerUI          10.1.7

## 🛣️ Documentação das Rotas
Todas as respostas de sucesso seguem o padrão de envelope:
'
{
  "dados_resposta": { ... },
  "timestamp_resposta": "dd/MM/yyyy HH:mm:ss",
  "tempo_da_resposta": "X ms"
}
'

  Método    Rota                          Descrição                                                 Parâmetros
> POST     /api/usuarios                  Cadastra um novo usuário e seus documentos.               Body: CadastroUsuarioDto
> GET      /api/usuarios                  Lista todos os usuários ativos ou filtra por e-mail.      Query: email (opcional)
> GET      /api/usuarios/{id}             Obtém detalhes de um usuário específico pelo GUID.        Path: id
> PUT      /api/usuarios/{id}             Atualiza usuário com validação de segurança.              Path: id; Query: email; Header: senha; Body: CadastroUsuarioDto
> DELETE   /api/usuarios/{id}             Inativa um usuário do sistema.                            Path: id; Body: ConfirmacaoOperacaoDto
> PATCH    /api/usuarios/ativar           Reativa um usuário inativo.                               Body: ConfirmacaoOperacaoDto
> GET      /api/usuarios/{id}/documentos  Retorna apenas a lista de documentos de um usuário.       Path: id (Guid)

## 📋 Exemplos de Objetos (DTOs)




## 📦 Como Instalar e Rodar
1. Clone o repositório: `git clone https://github.com/WiniciusDutra/SIGEUS.git`
2. Navegue até o diretório do projeto SIGEUS, onde está o arquivo `SIGEUS.csproj`
3. Instale as dependências: `dotnet restore`
4. Aplique as migrations para gerar o banco de dados em SqLite  `dotnet ef database update`
5. Dê build no projeto: `dotnet build`
6. Inicie a aplicação com: `dotnet watch run`


## 📄 Licença
Distribuído sob a licença MIT (https://mit-license.org/).
