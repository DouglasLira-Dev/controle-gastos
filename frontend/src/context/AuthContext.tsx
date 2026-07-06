import React, { createContext, useState, useContext, ReactNode, useEffect } from 'react';
import { authApi } from '../api/api';
import { AuthResponse, LoginRequest, RegisterRequest } from '../types';

interface AuthContextType {
  user: AuthResponse | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthResponse | null>(null);
  const [loading, setLoading] = useState(true);

  // 🔧 CORRIGIDO: Renomear variável para não colidir com a função
  useEffect(() => {
    const token = localStorage.getItem('token');
    const storedRefreshToken = localStorage.getItem('refreshToken'); // 👈 NOME DIFERENTE
    if (token && storedRefreshToken) {
      refreshToken(); // 👈 Agora chama a função, não a string!
    } else {
      setLoading(false);
    }
  }, []);

  const login = async (data: LoginRequest) => {
    const response = await authApi.login(data);
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    setUser(response);
  };

  const register = async (data: RegisterRequest) => {
    const response = await authApi.register(data);
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    setUser(response);
  };

  const logout = async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      await authApi.logout(refreshToken);
    }
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setUser(null);
  };

  const refreshToken = async () => {
    try {
      const storedRefreshToken = localStorage.getItem('refreshToken');
      if (!storedRefreshToken) {
        setLoading(false);
        return;
      }
      const response = await authApi.refresh(storedRefreshToken);
      localStorage.setItem('token', response.token);
      localStorage.setItem('refreshToken', response.refreshToken);
      setUser(response);
    } catch (error) {
      console.error('Erro ao renovar token:', error);
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      setUser(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthContext.Provider value={{
      user,
      isAuthenticated: !!user,
      loading,
      login,
      register,
      logout,
      refreshToken,
    }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};