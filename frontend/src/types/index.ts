// ============================================
// 🔒 TIPOS DO SISTEMA DE CONTROLE DE GASTOS
// ============================================

export enum TipoTransacao {
  Receita = 0,
  Despesa = 1
}

// ============================================
// PESSOA
// ============================================
export interface Pessoa {
  id: string;
  nome: string;
  idade: number;
  isMenorDeIdade: boolean;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
  transacoes: Transacao[];
}

export interface PessoaCreate {
  nome: string;
  idade: number;
}

// ============================================
// TRANSAÇÃO
// ============================================
export interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  tipoDescricao: string;
  pessoaId: string;
  nomePessoa: string;
  dataCriacao: string;
}

export interface TransacaoCreate {
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: string;
}

// ============================================
// TOTAIS
// ============================================
export interface TotalPorPessoa {
  pessoaId: string;
  nomePessoa: string;
  idade: number;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface TotalGeral {
  totalReceitasGeral: number;
  totalDespesasGeral: number;
  saldoLiquido: number;
  totaisPorPessoa: TotalPorPessoa[];
}

// ============================================
// 🔐 AUTENTICAÇÃO
// ============================================
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  username: string;
  email: string;
  role: string;
}