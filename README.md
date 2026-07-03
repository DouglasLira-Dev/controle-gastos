# 🏦 Sistema de Controle de Gastos Residenciais
---
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.2-61DAFB)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
---

## 📋 Sobre o Projeto

Sistema desenvolvido como desafio técnico para estágio em TI, implementando um controle de gastos residenciais com foco em:

- ✅ **Segurança** - Proteção de dados e práticas de segurança
- ✅ **Qualidade** - Código limpo, testável e documentado
- ✅ **Boas Práticas** - SOLID, Clean Code, Patterns
- ✅ **UX** - Interface intuitiva e responsiva
---

## 🎯 Funcionalidades

### 👤 Pessoas
- [ ] Cadastro de pessoas (Nome, Idade)
- [ ] Listagem de pessoas
- [ ] Deleção de pessoas (com remoção em cascata das transações)

### 💰 Transações
- [ ] Cadastro de transações (Descrição, Valor, Tipo, Pessoa)
- [ ] Listagem de transações
- [ ] Validação: menores de 18 anos só podem cadastrar despesas

### 📊 Consulta de Totais
- [ ] Totais por pessoa (Receitas, Despesas, Saldo)
- [ ] Totais gerais (Receitas totais, Despesas totais, Saldo líquido)

## 🛡️ Segurança

Este projeto implementa as seguintes medidas de segurança:

- **Backend**:
  - Validação de entrada com FluentValidation
  - Prevenção contra SQL Injection (Entity Framework)
  - Sanitização de dados
  - Tratamento seguro de erros (sem expor stack traces)
  - CORS configurado

- **Frontend**:
  - Validação de formulários com Zod
  - Sanitização de inputs
  - Proteção contra XSS
  - Environment variables para dados sensíveis
--- 

## 🛠️ Tecnologias

### Backend
- .NET 8
- Entity Framework Core 8
- SQL Server / PostgreSQL
- FluentValidation
- AutoMapper
- Swagger/OpenAPI
- xUnit (Testes)

### Frontend
- React 18
- TypeScript 5
- Vite
- Axios
- React Hook Form + Zod
- TailwindCSS / Material-UI
---

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK
- Node.js 18+
- SQL Server (ou Docker)
---

### Backend
```bash
# Clone o repositório
git clone https://github.com/seu-usuario/desafio-controle-gastos.git
cd desafio-controle-gastos/backend

# Restaurar pacotes
dotnet restore

# Configurar banco de dados
dotnet ef database update

# Executar API
dotnet run
```
---

### Frontend
```bash
cd frontend

# Instalar dependências
npm install

# Executar em desenvolvimento
npm run dev
```
---

### 📝 Status do Desenvolvimento
---
## ✅ Concluído

- Estrutura inicial do projeto
- Configuração do repositório
- Entidades Core (Pessoa, Transacao)
- DTOs e AutoMapper
---
## 🚧 Em Desenvolvimento

- Validações com FluentValidation
- Repository Pattern
- Services
- Controllers da API
- Frontend React
---

## ⏳ Pendente

- Testes unitários
- Documentação Swagger
- Dockerização
- CI/CD
---

### 📄 Licença
Este projeto está sob a licença MIT.