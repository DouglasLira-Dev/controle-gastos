import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Layout } from './components/Layout';

// Páginas
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import PessoasPage from './pages/PessoasPage';
import TransacoesPage from './pages/TransacoesPage';
import TotaisPage from './pages/TotaisPage';

function App() {
  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          
          <Route path="/" element={
            <ProtectedRoute>
              <Layout>
                <TotaisPage />
              </Layout>
            </ProtectedRoute>
          } />
          
          <Route path="/pessoas" element={
            <ProtectedRoute>
              <Layout>
                <PessoasPage />
              </Layout>
            </ProtectedRoute>
          } />
          
          <Route path="/transacoes" element={
            <ProtectedRoute>
              <Layout>
                <TransacoesPage />
              </Layout>
            </ProtectedRoute>
          } />
          
          <Route path="/totais" element={
            <ProtectedRoute>
              <Layout>
                <TotaisPage />
              </Layout>
            </ProtectedRoute>
          } />
        </Routes>
      </AuthProvider>
    </Router>
  );
}

export default App;