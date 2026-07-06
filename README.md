# 🏦 Sistema de Controle de Gastos Residenciais

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19.2-61DAFB)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-6.0-3178C6)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

## 📋 Sobre o Projeto

Sistema desenvolvido como desafio técnico, implementando um controle de gastos residenciais com cadastro de pessoas, lançamento de transações e consulta de totais. Além do escopo mínimo pedido, o projeto inclui autenticação JWT, autorização por papéis, rate limiting, logging estruturado e cabeçalhos de segurança.

## 🎯 Funcionalidades

### 👤 Pessoas
- [x] Cadastro de pessoas (Nome, Idade)
- [x] Listagem de pessoas (`GET /api/pessoas`) e busca por ID (`GET /api/pessoas/{id}`)
- [x] Deleção de pessoas com remoção em cascata das transações (`DELETE /api/pessoas/{id}`, restrito ao papel **Admin**)

### 💰 Transações
- [x] Cadastro de transações (Descrição, Valor, Tipo, Pessoa) — `POST /api/transacoes`
- [x] Listagem de todas as transações e por pessoa (`GET /api/transacoes`, `GET /api/transacoes/pessoa/{pessoaId}`)
- [x] Validação: menores de 18 anos só podem cadastrar transações do tipo **Despesa** (bloqueado em `TransacaoCreateValidator`)
- [x] Validação de existência da pessoa antes de criar a transação

### 📊 Consulta de Totais
- [x] Totais por pessoa (Receitas, Despesas, Saldo) e totais gerais do sistema — `GET /api/totais`

### 🔐 Autenticação (adicionada além do escopo original)
- [x] Registro (`POST /api/auth/register`), login (`POST /api/auth/login`), refresh (`POST /api/auth/refresh`) e logout (`POST /api/auth/logout`)
- [x] Todos os endpoints de Pessoas, Transações e Totais exigem token JWT (`[Authorize]`)
- [x] Deleção de pessoas exige papel `Admin`
- [x] Usuário admin criado automaticamente no primeiro `dotnet run` (ver seção **Credenciais padrão**)

## 🛡️ Segurança

- **Backend**:
  - Autenticação JWT (access token + refresh token) e autorização por papéis (`Admin`, `User`, `Guest`)
  - Validação de entrada com FluentValidation
  - Prevenção contra SQL Injection via Entity Framework Core
  - Sanitização de inputs (`InputSanitizer`)
  - Rate limiting global e por rota (criação/deleção) via `RateLimitingConfiguration`
  - Cabeçalhos de segurança HTTP (`SecurityHeadersMiddleware`)
  - Tratamento centralizado de erros sem exposição de stack trace (`ExceptionHandlingMiddleware`)
  - Logging estruturado com Serilog (console + arquivo em `logs/`)
  - CORS restrito às origens do frontend
- **Frontend**:
  - Validação de formulários com Zod
  - Token JWT enviado automaticamente via interceptor do Axios
  - Rotas protegidas (`ProtectedRoute`)

> ⚠️ **Nota:** o requisito original do desafio não pedia autenticação. Ela foi adicionada como diferencial, mas isso significa que **nenhum endpoint funciona sem login primeiro** — veja a seção "Como testar" abaixo.

## 🛠️ Tecnologias

### Backend
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core (SQLite)
- FluentValidation
- AutoMapper
- JWT Bearer Authentication
- Serilog
- Swagger/OpenAPI

### Frontend
- React 19
- TypeScript 6
- Vite
- Axios
- React Hook Form + Zod
- TailwindCSS

## 🚀 Como Executar

### Pré-requisitos
- .NET 10 SDK
- Node.js 18+

### Backend

O projeto usa **SQLite** — não é necessário instalar SQL Server, PostgreSQL nem Docker. O banco (`DesafioGastos.db`) é criado automaticamente na primeira execução.

```bash
cd backend

# Configurar a chave JWT (obrigatório antes do primeiro dotnet run)
dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-bem-longa-com-pelo-menos-32-caracteres" --project DesafioControleGastos.API
dotnet user-secrets set "Jwt:Issuer" "DesafioControleGastos" --project DesafioControleGastos.API
dotnet user-secrets set "Jwt:Audience" "DesafioControleGastos" --project DesafioControleGastos.API

# Restaurar pacotes e executar
dotnet restore
dotnet run --project DesafioControleGastos.API
```

A API sobe em `http://localhost:5139` (Swagger em `/swagger`).

> ⚠️ **Importante:** sem as chaves `Jwt:Key`/`Jwt:Issuer`/`Jwt:Audience` configuradas (via user secrets ou variável de ambiente), a aplicação lança exceção ao iniciar e não sobe. Essas chaves não são versionadas no repositório por segurança.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Acesse `http://localhost:5173`. O Vite já está configurado com proxy de `/api` para `http://localhost:5139`.

## 🔑 Credenciais padrão (seed automático)

No primeiro `dotnet run`, além de uma pessoa e transações de exemplo, um usuário administrador é criado automaticamente:

| Usuário | Senha       | Papel |
|---------|-------------|-------|
| `admin` | `Admin123!` | Admin |

Use essas credenciais em `/api/auth/login` para obter um token com permissão de deletar pessoas. Usuários criados via `/api/auth/register` recebem o papel `User` por padrão e **não conseguem deletar pessoas** (apenas criar/listar).

## 🧪 Como testar o fluxo completo

1. `POST /api/auth/login` com `admin` / `Admin123!` → copiar o `token` retornado.
2. No Swagger, clicar em **Authorize** e informar `Bearer {token}`.
3. Testar `POST /api/pessoas`, `POST /api/transacoes`, `GET /api/totais` e `DELETE /api/pessoas/{id}`.

## 📌 Estado do projeto e pontos em aberto

- **Testes automatizados**: o projeto `DesafioControleGastos.Tests` existe na solução, mas ainda não contém casos de teste implementados. Próximo passo natural é cobrir `TransacaoCreateValidator` (regra do menor de idade), `PessoaCreateValidator` e o cálculo de totais.
- **Migrations**: existem migrations geradas em `Infra/Migrations`, mas a inicialização do banco em `Program.cs` usa `Database.EnsureCreated()`. Em produção, o recomendado seria usar `Database.Migrate()` para aplicar as migrations versionadas.
- **Docker/CI-CD**: não implementados.

## 📄 Licença

Este projeto está sob a licença MIT.