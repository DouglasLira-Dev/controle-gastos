import axios from 'axios';
import type { 
  Pessoa, 
  PessoaCreate, 
  Transacao, 
  TransacaoCreate, 
  TotalGeral,
  LoginRequest,
  RegisterRequest,
  AuthResponse
} from '../types';

// ============================================
// 🔒 CONFIGURAÇÃO DA API
// ============================================

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// 🔒 Interceptor para adicionar token em todas as requisições
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// ============================================
// 🔐 ENDPOINTS DE AUTENTICAÇÃO
// ============================================

export const authApi = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await api.post('/auth/login', data);
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post('/auth/register', data);
    return response.data;
  },

  refresh: async (refreshToken: string): Promise<AuthResponse> => {
    const response = await api.post('/auth/refresh', { refreshToken });
    return response.data;
  },

  logout: async (refreshToken: string): Promise<void> => {
    await api.post('/auth/logout', refreshToken);
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
  },
};

// ============================================
// 👤 ENDPOINTS DE PESSOAS
// ============================================

export const pessoasApi = {
  getAll: async (): Promise<Pessoa[]> => {
    const response = await api.get('/pessoas');
    return response.data;
  },

  getById: async (id: string): Promise<Pessoa> => {
    const response = await api.get(`/pessoas/${id}`);
    return response.data;
  },

  create: async (data: PessoaCreate): Promise<Pessoa> => {
    const response = await api.post('/pessoas', data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/pessoas/${id}`);
  },
};

// ============================================
// 💰 ENDPOINTS DE TRANSAÇÕES
// ============================================

export const transacoesApi = {
  getAll: async (): Promise<Transacao[]> => {
    const response = await api.get('/transacoes');
    return response.data;
  },

  getByPessoa: async (pessoaId: string): Promise<Transacao[]> => {
    const response = await api.get(`/transacoes/pessoa/${pessoaId}`);
    return response.data;
  },

  create: async (data: TransacaoCreate): Promise<Transacao> => {
    const response = await api.post('/transacoes', data);
    return response.data;
  },
};

// ============================================
// 📊 ENDPOINTS DE TOTAIS
// ============================================

export const totaisApi = {
  getTotais: async (): Promise<TotalGeral> => {
    const response = await api.get('/totais');
    return response.data;
  },
};

export default api;